# GAMES101作业6 - 加速结构

> 代码地址：
>
> https://github.com/Li-Kira/CodeLib/tree/main/CG/GAMES101/Homework06-%E5%8A%A0%E9%80%9F%E7%BB%93%E6%9E%84

## 目标

- Ray-Bounding Volume包围盒求交：正确实现光线与包围盒求交函数。
- BVH 查找：正确实现 BVH 加速的光线与场景求交



## 补充框架

**Render() in Renderer.cpp:**

框架里面已经计算了x和y，因此我们只需要补充剩下的生成光线的步骤即可，代码如下：

```c++
Vector3f dir = normalize(Vector3f(x, y, -1)); 
framebuffer[m++] = scene.castRay(Ray(eye_pos, dir), 0);
```

**Triangle::getIntersection in Triangle.hpp:**

> 这段代码初看没什么头绪，参考了以下文档：https://blueflame.org.cn/archives/437

这段代码需要补充的地方是给inter赋值，代码如下：

```c++
inline Intersection Triangle::getIntersection(Ray ray)
{
    Intersection inter;

    if (dotProduct(ray.direction, normal) > 0)
        return inter;
    double u, v, t_tmp = 0;
    Vector3f pvec = crossProduct(ray.direction, e2);
    double det = dotProduct(e1, pvec);
    if (fabs(det) < EPSILON)
        return inter;

    double det_inv = 1. / det;
    Vector3f tvec = ray.origin - v0;
    u = dotProduct(tvec, pvec) * det_inv;
    if (u < 0 || u > 1)
        return inter;
    Vector3f qvec = crossProduct(tvec, e1);
    v = dotProduct(ray.direction, qvec) * det_inv;
    if (v < 0 || u + v > 1)
        return inter;
    t_tmp = dotProduct(e2, qvec) * det_inv;

    // TODO find ray triangle intersection
    if (t_tmp < 0)
        return inter;
    inter.distance = t_tmp;//光线经过的时间
    inter.happened = true;//是否与三角形相交
    inter.m = m;//三角形的材质
    inter.obj = this;//Triangle继承了Object，重写了virtual Intersection getIntersection(Ray _ray)，三角形调用getIntersection(Ray _ray)，intersection自然记录下当前在相交的三角形，所以用this
    inter.normal = normal;//三角形面的法线
    inter.coords = ray(t_tmp);//Vector3f operator()(double t) const{return origin+direction*t;} in Ray.hpp, coords表示相交点的坐标

    return inter;
}
```



## Ray-Bounding Volume包围盒求交

> 如果包围盒没有算好，可能会出现全蓝的错误结果。

在`Bounds3.hpp`中的**IntersectP**方法中，我们需要根据**t_enter**和**t_exit**直接的大小关系返回是否与该包围盒相交。

对于3D盒子，**t_enter = max{t_min}，t_exit = min{t_max}**，因此我们只需要求出xyz三对面的**t_min**和**t_max**即可，而三对面的**t_min**和**t_max**可以用轴对齐包围盒求t公式求解。

对于平行与x轴的面，公式如下：
$$
t = \frac{p'_x - o_x}{d_x}
$$
其他公式由此类推即可，以下是运行耗时14s的代码：

```c++
inline bool Bounds3::IntersectP(const Ray& ray, const Vector3f& invDir,
                                const std::array<int, 3>& dirIsNeg) const
{
    // invDir: ray direction(x,y,z), invDir=(1.0/x,1.0/y,1.0/z), use this because Multiply is faster that Division
    // dirIsNeg: ray direction(x,y,z), dirIsNeg=[int(x>0),int(y>0),int(z>0)], use this to simplify your logic
    // TODO test if ray bound intersects
    
    double t_enter = std::numeric_limits<double>::lowest();
    double t_exit = std::numeric_limits<double>::max();

    //对三对面进行判断
    for (int i = 0; i < 3; i++)
    {
        auto t_min = (pMin[i] - ray.origin[i]) * invDir[i];
        auto t_max = (pMax[i] - ray.origin[i]) * invDir[i];
        if (!dirIsNeg[i])
            std::swap(t_min, t_max);

        t_enter = std::max(t_min, t_enter);
        t_exit = std::min(t_max, t_exit);
    }

    float eps = 0.00001;
    if (t_enter < t_exit && t_exit + eps > 0)
        return true;
    else
        return false;
}
```

以下是运行耗时2min的代码：

```c++
inline bool Bounds3::IntersectP(const Ray& ray, const Vector3f& invDir,
                                const std::array<int, 3>& dirIsNeg) const
{
    // invDir: ray direction(x,y,z), invDir=(1.0/x,1.0/y,1.0/z), use this because Multiply is faster that Division
    // dirIsNeg: ray direction(x,y,z), dirIsNeg=[int(x>0),int(y>0),int(z>0)], use this to simplify your logic
    // TODO test if ray bound intersects
    
    double t_enter = std::numeric_limits<double>::lowest();
    double t_exit = std::numeric_limits<double>::max();

	auto x_min = (pMin.x - ray.origin.x) * invDir[0];
	auto x_max = (pMax.x - ray.origin.x) * invDir[0];
	if (!dirIsNeg[0])
    	std::swap(x_min, x_max);

	auto y_min = (pMin.y - ray.origin.y) * invDir[1];
	auto y_max = (pMax.y - ray.origin.y) * invDir[1];
	if (!dirIsNeg[1])
    	std::swap(y_min, y_max);

	auto z_min = (pMin.z - ray.origin.z) * invDir[2];
	auto z_max = (pMax.z - ray.origin.z) * invDir[2];
	if (!dirIsNeg[2])
    	std::swap(z_min, z_max);

	t_enter = std::max(x_min, std::max(y_min, z_min));
	t_exit = std::min(x_max, std::max(y_max, z_max));
    
    float eps = 0.00001;
    if (t_enter < t_exit && t_exit + eps > 0)
        return true;
    else
        return false;
}
```



## BVH 查找

根据之前的伪代码写以下的代码即可

需要注意，**IntersectP**里面的参数有一项` const std::array<int, 3>& dirIsNeg`，需要用以下格式输入：

`{ray.direction.x > 0, ray.direction.y > 0, ray.direction.z > 0}`

```c++
Intersection BVHAccel::getIntersection(BVHBuildNode* node, const Ray& ray) const
{
    // TODO Traverse the BVH to find intersection

    if (node->bounds.IntersectP(ray, ray.direction_inv, {ray.direction.x > 0, ray.direction.y > 0, ray.direction.z > 0}))
    {
        if (node->left == nullptr && node->right == nullptr)
        {
            return node->object->getIntersection(ray);
        }
        else
        {
            auto hit1 = getIntersection(node->left, ray);
            auto hit2 = getIntersection(node->right, ray);
            return hit1.distance < hit2.distance ? hit1 : hit2;
        }
    }
    return Intersection();
}
```



结果如图：

![image-20230903112625573](F:\Typora_Image\image-20230903112625573.png)