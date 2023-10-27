# GAMES101作业2 - 光栅化

> 代码地址：
>
> https://github.com/Li-Kira/CodeLib/tree/main/CG/GAMES101/Homework02-%E5%85%89%E6%A0%85%E5%8C%96

## 目标

- 正确实现三角形栅格化算法。
- 正确测试点是否在三角形内。
- 正确实现 z-buffer 算法, 将三角形按顺序画在屏幕上。
- 用 super-sampling 处理 Anti-aliasing （MSAA）



## 判断是否在三角形内

```c++
static bool insideTriangle(float x, float y, const Vector3f* _v)
{   
    // TODO : Implement this function to check if the point (x, y) is inside the triangle represented by _v[0], _v[1], _v[2]

    float checkPoint_x = x;
    float checkPoint_y = y;

    Eigen::Vector2f AB = Eigen::Vector2f(_v[1].x() - _v[0].x(), _v[1].y() - _v[0].y());
    Eigen::Vector2f BC = Eigen::Vector2f(_v[2].x() - _v[1].x(), _v[2].y() - _v[1].y());
    Eigen::Vector2f CA = Eigen::Vector2f(_v[0].x() - _v[2].x(), _v[0].y() - _v[2].y());
    Eigen::Vector2f AP = Eigen::Vector2f(checkPoint_x - _v[0].x(), checkPoint_y - _v[0].y());
    Eigen::Vector2f BP = Eigen::Vector2f(checkPoint_x - _v[1].x(), checkPoint_y - _v[1].y());
    Eigen::Vector2f CP = Eigen::Vector2f(checkPoint_x - _v[2].x(), checkPoint_y - _v[2].y());

    float cross_AB_AP = AB.x() * AP.y() - AB.y() * AP.x();
    float cross_BC_BP = BC.x() * BP.y() - BC.y() * BP.x();
    float cross_CA_CP = CA.x() * CP.y() - CA.y() * CP.x();

    if (cross_AB_AP > 0 && cross_BC_BP > 0 && cross_CA_CP > 0 ||
        cross_AB_AP < 0 && cross_BC_BP < 0 && cross_CA_CP < 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}
```



## 光栅化三角形

```c++
void rst::rasterizer::rasterize_triangle(const Triangle& t) {
auto v = t.toVector4();

// TODO : Find out the bounding box of current triangle.
// iterate through the pixel and find if the current pixel is inside the triangle

// If so, use the following code to get the interpolated z value.
//auto[alpha, beta, gamma] = computeBarycentric2D(x, y, t.v);
//float w_reciprocal = 1.0/(alpha / v[0].w() + beta / v[1].w() + gamma / v[2].w());
//float z_interpolated = alpha * v[0].z() / v[0].w() + beta * v[1].z() / v[1].w() + gamma * v[2].z() / v[2].w();
//z_interpolated *= w_reciprocal;

// TODO : set the current pixel (use the set_pixel function) to the color of the triangle (use getColor function) if it should be painted.


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
                Eigen::Vector3f point = Eigen::Vector3f(x, y, z_interpolated);
                set_pixel(point, t.getColor());
                depth_buf[get_index(x, y)] = z_interpolated;
            }
        }
    }
}
```



## 附加题 - 实现MSAA

首先，需要在`rasterizer.cpp`中，添加深度缓存和颜色缓存：

```c++
std::vector<Eigen::Vector3f> sampleList_frame_buf;
std::vector<float> sampleList_depth_buf;
```

同时需要对他们进行初始化：

```c++
void rst::rasterizer::clear(rst::Buffers buff)
{
    if ((buff & rst::Buffers::Color) == rst::Buffers::Color)
    {
        std::fill(frame_buf.begin(), frame_buf.end(), Eigen::Vector3f{0, 0, 0});
        std::fill(sampleList_frame_buf.begin(), sampleList_frame_buf.end(), Eigen::Vector3f{ 0, 0, 0 });
    }
    if ((buff & rst::Buffers::Depth) == rst::Buffers::Depth)
    {
        std::fill(depth_buf.begin(), depth_buf.end(), std::numeric_limits<float>::infinity());
        std::fill(sampleList_depth_buf.begin(), sampleList_depth_buf.end(), std::numeric_limits<float>::infinity());
    }
}

rst::rasterizer::rasterizer(int w, int h) : width(w), height(h)
{
    frame_buf.resize(w * h);
    depth_buf.resize(w * h);

    sampleList_frame_buf.resize(w * h * 4);
    sampleList_depth_buf.resize(w * h * 4);
}
```

然后修改光栅化类：

```c++
//以下是实现MSAA的版本
struct SamplePoint
{
    float p_x;
    float p_y;
    int isInsideTriangle;

    SamplePoint(float _x, float _y, int _isInsideTriangle)
    {
        p_x = _x;
        p_y = _y;
        isInsideTriangle = _isInsideTriangle;
    }
};

//遍历boundbox，渲染三角面
for (int x = x_min; x < x_max; x++)
{
    for (int y = y_min; y < y_max; y++)
    {
        std::vector<SamplePoint> samplePoint;

        samplePoint.push_back(SamplePoint(x + 0.25, y + 0.25, insideTriangle(x + 0.25, y + 0.25, t.v) == true ? 1 : 0));
        samplePoint.push_back(SamplePoint(x + 0.25, y + 0.75, insideTriangle(x + 0.25, y + 0.75, t.v) == true ? 1 : 0));
        samplePoint.push_back(SamplePoint(x + 0.75, y + 0.25, insideTriangle(x + 0.75, y + 0.25, t.v) == true ? 1 : 0));
        samplePoint.push_back(SamplePoint(x + 0.75, y + 0.75, insideTriangle(x + 0.75, y + 0.75, t.v) == true ? 1 : 0));

        float minDep = INT_MAX;
        float avg_color = 0;

        //遍历定义的四个样本点
        for (int i = 0; i < samplePoint.size(); i++)
        {
            //如果当前点在三角形内，则更新深度缓冲区
            if (samplePoint[i].isInsideTriangle > 0)
            {
                auto result = computeBarycentric2D(samplePoint[i].p_x, samplePoint[i].p_y, t.v);
                float alpha, beta, gamma;
                std::tie(alpha, beta, gamma) = result;
                float w_reciprocal = 1.0 / (alpha / v[0].w() + beta / v[1].w() + gamma / v[2].w());
                float z_interpolated = alpha * v[0].z() / v[0].w() + beta * v[1].z() / v[1].w() + gamma * v[2].z() / v[2].w();
                z_interpolated *= w_reciprocal;

                minDep = std::min(z_interpolated, minDep);
                avg_color += 0.25;
            }
        }

        if (minDep < depth_buf[get_index(x, y)])
        {
            Eigen::Vector3f point = Eigen::Vector3f(x, y, 1);
            set_pixel(point, t.getColor() * avg_color);
            depth_buf[get_index(x, y)] = minDep;
        }

    }
}
```





![作业2](F:\Typora_Image\作业2.png)



放大看局部细节：

![image-20230830170200301](F:\Typora_Image\image-20230830170200301.png)



![image-20230830170131715](F:\Typora_Image\image-20230830170131715.png)



