# GAMES101作业3 - 着色

> 代码地址：
>
> https://github.com/Li-Kira/CodeLib/tree/main/CG/GAMES101/Homework03-%E7%9D%80%E8%89%B2
>
> 如何在vs中包含文件include folder：
>
> https://www.youtube.com/watch?v=PjxRNzyaxiw

## 配置环境

在开始作业之前，需要配置一下环境。

- 项目->属性->C++->语言->C++语言标准 选择`ISO C++17标准`

![image-20230824181758053](F:\Typora_Image\image-20230824181758053.png)

- 修改模型文件路径

在`main.cpp`中，将路径修改为我们存储模型的路径

```c++
std::string filename = "output.png";
objl::Loader Loader;
std::string obj_path = "E:/Code/CG/CGHomework/Homework03/models/spot/";

// Load .obj File
bool loadout = Loader.LoadFile("E:/Code/CG/CGHomework/Homework03/models/spot/spot_triangulated_good.obj");
```

- 更改着色器

在`main.cpp`中，由于默认着色器是`phong_fragment_shader`，如果没有实现，想要观察normal shader的效果，可以将其中的参数求改为`normal_fragment_shader`。

```c++
std::function<Eigen::Vector3f(fragment_shader_payload)> active_shader = normal_fragment_shader;
```

> 作业文档有提示可以使用`./Rasterizer output.png normal`，用输入参数的方式更改着色器，使用visual studio想要用参数调试可以使用以下方法：
>
> 项目->属性->调试中的`命令参数`可以填入我们运行时需要的参数。







## 目标

- 在光栅化三角形的函数中实现插值算法
- 添加投影矩阵
- 在`Phong Shader`中实现Blinn-Phong光照模型
- 在`Bump Shader`中实现凹凸映射
- 在`Displacement Shader`中实现位移纹理
- （附加题）尝试更多模型
- （附加题）使用双线性插值进行纹理采样：在 Texture 类中实现一个新方法`getColorBilinear`





## 实现插值算法

以下是完整代码：

```c++
//Screen space rasterization
void rst::rasterizer::rasterize_triangle(const Triangle& t, const std::array<Eigen::Vector3f, 3>& view_pos) 
{
    // TODO: From your HW3, get the triangle rasterization code.
    // TODO: Inside your rasterization loop:
    //    * v[i].w() is the vertex view space depth value z.
    //    * Z is interpolated view space depth for the current pixel
    //    * zp is depth between zNear and zFar, used for z-buffer

    // float Z = 1.0 / (alpha / v[0].w() + beta / v[1].w() + gamma / v[2].w());
    // float zp = alpha * v[0].z() / v[0].w() + beta * v[1].z() / v[1].w() + gamma * v[2].z() / v[2].w();
    // zp *= Z;

    // TODO: Interpolate the attributes:
    // auto interpolated_color
    // auto interpolated_normal
    // auto interpolated_texcoords
    // auto interpolated_shadingcoords

    // Use: fragment_shader_payload payload( interpolated_color, interpolated_normal.normalized(), interpolated_texcoords, texture ? &*texture : nullptr);
    // Use: payload.view_pos = interpolated_shadingcoords;
    // Use: Instead of passing the triangle's color directly to the frame buffer, pass the color to the shaders first to get the final color;
    // Use: auto pixel_color = fragment_shader(payload);

    auto v = t.toVector4();

    //bounding box的实现
    float x_max = v[0].x(), x_min = v[0].x(), y_max = v[0].y(), y_min = v[0].y();

    for (auto i : v)
    {
        if (i[0] < x_min)
            x_min = i[0];
        if (i[0] > x_max)
            x_max = i[0];
        if (i[1] < y_min)
            y_min = i[1];
        if (i[1] > y_max)
            y_max = i[1];
    }

    
    //以下是未进行抗锯齿处理的版本
    for (int x = x_min; x < x_max; x++)
    {
        for (int y = y_min; y < y_max; y++)
        {
            bool isInsideTriangle = insideTriangle(x, y, t.v);
            if (isInsideTriangle)
            {
                auto result = computeBarycentric2D(x + 0.5, y + 0.5, t.v);
                float alpha, beta, gamma;
                std::tie(alpha, beta, gamma) = result;
                float w_reciprocal = 1.0 / (alpha / v[0].w() + beta / v[1].w() + gamma / v[2].w());
                float z_interpolated = alpha * v[0].z() / v[0].w() + beta * v[1].z() / v[1].w() + gamma * v[2].z() / v[2].w();
                z_interpolated *= w_reciprocal;

                if (z_interpolated < depth_buf[get_index(x, y)])
                {
                    auto interpolated_color = interpolate(alpha, beta, gamma, t.color[0], t.color[1], t.color[2], 1);
                    auto interpolated_normal = interpolate(alpha, beta, gamma, t.normal[0], t.normal[1], t.normal[2], 1);
                    auto interpolated_texcoords = interpolate(alpha, beta, gamma, t.tex_coords[0], t.tex_coords[1], t.tex_coords[2], 1);
                    auto interpolated_shadingcoords = interpolate(alpha, beta, gamma, view_pos[0], view_pos[0], view_pos[0], 1);

                    fragment_shader_payload payload(interpolated_color, interpolated_normal.normalized(), interpolated_texcoords, texture ? &*texture : nullptr);
                    payload.view_pos = interpolated_shadingcoords;
                    auto pixel_color = fragment_shader(payload);
                    Vector2i vertex;
                    vertex << x, y;

                    depth_buf[get_index(x, y)] = z_interpolated;
                    frame_buf[get_index(x, y)] = fragment_shader(payload);
                    set_pixel(vertex, pixel_color);
                }
            }
        }
    }
 
}
```





![image-20230824181952294](F:\Typora_Image\image-20230824181952294.png)



## Blinn-Phong

```c++
Eigen::Vector3f phong_fragment_shader(const fragment_shader_payload& payload)
{
    Eigen::Vector3f ka = Eigen::Vector3f(0.005, 0.005, 0.005);
    Eigen::Vector3f kd = payload.color;
    Eigen::Vector3f ks = Eigen::Vector3f(0.7937, 0.7937, 0.7937);

    auto l1 = light{{20, 20, 20}, {500, 500, 500}};
    auto l2 = light{{-20, 20, 0}, {500, 500, 500}};

    std::vector<light> lights = {l1, l2};
    Eigen::Vector3f amb_light_intensity{10, 10, 10};
    Eigen::Vector3f eye_pos{0, 0, 10};

    float p = 150;

    Eigen::Vector3f color = payload.color;
    Eigen::Vector3f point = payload.view_pos;
    Eigen::Vector3f normal = payload.normal;

    Eigen::Vector3f result_color = {0, 0, 0};
    for (auto& light : lights)
    {
        // TODO: For each light source in the code, calculate what the *ambient*, *diffuse*, and *specular* 
        // components are. Then, accumulate that result on the *result_color* object.

        auto l = (light.position - point).normalized();
        auto v = (eye_pos - point).normalized();
        auto h = (v + l).normalized();
        double r2 = (light.position - point).dot(light.position - point);
        
        auto diffuse = kd.cwiseProduct((light.intensity / r2) * std::max(0.0f, normal.dot(l)));
        auto specular = ks.cwiseProduct((light.intensity / r2) * std::pow(std::max(0.0f, normal.dot(h)), p));
        
        result_color += (diffuse + specular);
    }

    auto ambient = ka.cwiseProduct(amb_light_intensity);
    result_color += ambient;

    return result_color * 255.f;
}
```

> 需要注意的是，ambient只需要计算一次。





![image-20230825181557386](F:\Typora_Image\image-20230825181557386.png)





## Texture mapping

在编写纹理映射的Shader之前，需要考虑纹理的越界问题，以下是对`Texture.hpp`的修改。

```c++
Eigen::Vector3f getColor(float u, float v)
{
    if (u < 0) u = 0;
    if (u > 1) u = 1;
    if (v < 0) v = 0;
    if (v > 1) v = 1;

    auto u_img = u * width;
    auto v_img = (1 - v) * height;

    auto color = image_data.at<cv::Vec3b>(v_img, u_img);
    return Eigen::Vector3f(color[0], color[1], color[2]);
}
```



大致的代码跟`phong shader`的差不多，多了从纹理中获取颜色的过程，即以下的代码。

```c++
Eigen::Vector3f return_color = {0, 0, 0};
if (payload.texture)
{
    // TODO: Get the texture value at the texture coordinates of the current fragment
    return_color = payload.texture->getColor(payload.tex_coords.x(), payload.tex_coords.y());
}
```



完整代码如下：

```c++
Eigen::Vector3f texture_fragment_shader(const fragment_shader_payload& payload)
{
    Eigen::Vector3f return_color = {0, 0, 0};
    if (payload.texture)
    {
        // TODO: Get the texture value at the texture coordinates of the current fragment
        return_color = payload.texture->getColor(payload.tex_coords.x(), payload.tex_coords.y());
    }
    Eigen::Vector3f texture_color;
    texture_color << return_color.x(), return_color.y(), return_color.z();

    Eigen::Vector3f ka = Eigen::Vector3f(0.005, 0.005, 0.005);
    Eigen::Vector3f kd = texture_color / 255.f;
    Eigen::Vector3f ks = Eigen::Vector3f(0.7937, 0.7937, 0.7937);

    auto l1 = light{{20, 20, 20}, {500, 500, 500}};
    auto l2 = light{{-20, 20, 0}, {500, 500, 500}};

    std::vector<light> lights = {l1, l2};
    Eigen::Vector3f amb_light_intensity{10, 10, 10};
    Eigen::Vector3f eye_pos{0, 0, 10};

    float p = 150;

    Eigen::Vector3f color = texture_color;
    Eigen::Vector3f point = payload.view_pos;
    Eigen::Vector3f normal = payload.normal;

    Eigen::Vector3f result_color = {0, 0, 0};

    for (auto& light : lights)
    {
        // TODO: For each light source in the code, calculate what the *ambient*, *diffuse*, and *specular* 
        // components are. Then, accumulate that result on the *result_color* object.

        auto l = (light.position - point).normalized();
        auto v = (eye_pos - point).normalized();
        auto h = (v + l).normalized();
        double r2 = (light.position - point).dot(light.position - point);

        auto diffuse = kd.cwiseProduct((light.intensity / r2) * std::max(0.0f, normal.dot(l)));
        auto specular = ks.cwiseProduct((light.intensity / r2) * std::pow(std::max(0.0f, normal.dot(h)), p));

        result_color += (diffuse + specular);
    }

    auto ambient = ka.cwiseProduct(amb_light_intensity);
    result_color += ambient;

    return result_color * 255.f;
}
```



![image-20230825190612168](F:\Typora_Image\image-20230825190612168.png)





## Bump mapping

凹凸纹理需要对发现normal进行的数值进行更改，读取凹凸纹理内的uv值。

关于TBN的计算详解在之后的路径追踪，目前根据注释写就可以了。

u和v是纹理坐标的x和y，w和h是纹理的宽度和高度。

```c++
float x = normal.x();
float y = normal.y();
float z = normal.z();
Eigen::Vector3f t = Eigen::Vector3f(x * y / sqrt(x * x + z * z), sqrt(x * x + z * z), z * y / sqrt(x * x + z * z));
Eigen::Vector3f b = normal.cross(t);
Eigen::Matrix3f TBN;
TBN.col(0) = t;
TBN.col(1) = b;
TBN.col(2) = normal;

float u = payload.tex_coords.x();
float v = payload.tex_coords.y();
float w = payload.texture->width;
float h = payload.texture->height;

float dU = kh * kn * (payload.texture->getColor(u + 1.0 / w, v).norm() - payload.texture->getColor(u, v).norm());
float dV = kh * kn * (payload.texture->getColor(u, v + 1.0 / h).norm() - payload.texture->getColor(u, v).norm());
Eigen::Vector3f ln(-dU, -dV, 1);
normal = (TBN * ln).normalized();

Eigen::Vector3f result_color = {0, 0, 0};
result_color = normal;

return result_color * 255.f;
```





![image-20230825194422300](F:\Typora_Image\image-20230825194422300.png)





## Displacement mapping

位移纹理需要在凹凸纹理的基础上对point进行更改。

```c++
...
Eigen::Vector3f ln(-dU, -dV, 1);

//位移贴图需要加这一段
point += kn * normal * payload.texture->getColor(u, v).norm();
normal = (TBN * ln).normalized();
...
```



![image-20230825194859615](F:\Typora_Image\image-20230825194859615.png)







## 附加题 - 双线性插值

```c++
Eigen::Vector3f getColorBilinear(float u, float v)
{
    if (u < 0) u = 0;
    if (u > 1) u = 1;
    if (v < 0) v = 0;
    if (v > 1) v = 1;

    auto u_img = u * width;
    auto v_img = (1 - v) * height;

    float u_floor = std::floor(u_img);
    float v_floor = std::floor(v_img);

    auto u00 = image_data.at<cv::Vec3b>(v_floor, u_floor);
    auto u01 = image_data.at<cv::Vec3b>(v_floor + 1, u_floor);
    auto u11 = image_data.at<cv::Vec3b>(v_floor, u_floor + 1);
    auto u10 = image_data.at<cv::Vec3b>(v_floor + 1, u_floor + 1);

    float s_u = u_img - u_floor;
    float s_v = v_img - v_floor;

    auto u0 = u00 + s_u * (u10 - u00);
    auto u1 = u01 + s_u * (u11 - u01);

    auto interpolated_color = u0 + s_v * (u1 - u0);
    return Eigen::Vector3f(interpolated_color[0], interpolated_color[1], interpolated_color[2]);
}
```



![作业3-双线性插值](F:\Typora_Image\作业3-双线性插值.png)

