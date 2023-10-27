# GAMES101作业7 - 路径追踪

> 代码地址：
>
> https://github.com/Li-Kira/CodeLib/tree/main/CG/GAMES101/Homework07-%E8%B7%AF%E5%BE%84%E8%BF%BD%E8%B8%AA



## 目标

- 迁移代码
- Path Tracing：正确实现 Path Tracing 算法
- （附加题）多线程：将多线程应用在 Ray Generation 上
- （附加题）Microfacet：正确实现 Microfacet 材质



## 迁移代码

**Triangle.hpp:**

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

	if (t_tmp < 0)
		return inter;

    // TODO find ray triangle intersection
    inter.distance = t_tmp;//光线经过的时间
    inter.happened = true;//是否与三角形相交
    inter.m = m;//三角形的材质
    inter.obj = this;//Triangle继承了Object，重写了virtual Intersection getIntersection(Ray _ray)，三角形调用getIntersection(Ray _ray)，intersection自然记录下当前在相交的三角形，所以用this
    inter.normal = normal;//三角形面的法线
    inter.coords = ray(t_tmp);//Vector3f operator()(double t) const{return origin+direction*t;} in Ray.hpp, coords表示相交点的坐标

    return inter;
}
```

> 加了这一条，没有黑点了
>
> 	if (t_tmp < 0)
> 		return inter;





**Bounds3.hpp**

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

    if (t_enter <= t_exit && t_exit >= 0)
        return true;
    else
        return false;
}
```



**BVH.cpp**

```c++
Intersection BVHAccel::getIntersection(BVHBuildNode* node, const Ray& ray) const
{
    // TODO Traverse the BVH to find intersection
    if (node->bounds.IntersectP(ray, ray.direction_inv, { ray.direction.x > 0, ray.direction.y > 0, ray.direction.z > 0 }))
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







## Path Tracing 算法

以下是作业pdf中的伪代码：

```c++
shade (p, wo)
    sampleLight (inter , pdf_light)
    Get x, ws , NN , emit from inter
    Shoot a ray from p to x
    If the ray is not blocked in the middle
        L_dir = emit * eval(wo , ws , N) * dot(ws , N) * dot(ws , NN) / |x-p|^2 / pdf_light
    
    L_indir = 0.0
    Test Russian Roulette with probability RussianRoulette
    wi = sample (wo , N)
    Trace a ray r(p, wi)
    If ray r hit a non - emitting object at q
        L_indir = shade (q, -wi) * eval (wo , wi , N) * dot(wi , N) / pdf(wo , wi , N) / RussianRoulette
    
    Return L_dir + L_indir
```

根据作业pdf中的伪代码敲：

```c++
Vector3f Scene::castRay(const Ray &ray, int depth) const
{
    // TO DO Implement Path Tracing Algorithm here
    Intersection intersection = intersect(ray);

    //没有碰到物体，而是碰到背景的情况
    if (!intersection.happened)
    {
        return Vector3f(0);
    }

    //打到光源的情况，说明只计算自发光项
    if (intersection.m->hasEmission())
    {
        if (depth == 0)
            return intersection.m->getEmission();
        else
            return Vector3f(0);
    }

    // 射到物体的情况
    Vector3f L_dir(0), L_indir(0);

    //对场景中的光源均匀采样，得到pdf
    Intersection inter_light;
    float pdf = 0.f;
    sampleLight(inter_light, pdf);

    Vector3f p = intersection.coords;
    Vector3f x = inter_light.coords;
    Vector3f wo = ray.direction;
    Vector3f ws = (x - p).normalized();
    Vector3f N = intersection.normal.normalized();
    Vector3f NN = inter_light.normal.normalized();
    Vector3f emit = inter_light.emit;
    float distance = (x - p).norm();

    //从p点向x点射出光线
    //修正的p点
    Vector3f fixed_p = (dotProduct(ray.direction, N) < 0) ?
        p + N * EPSILON :
        p - N * EPSILON;

    Ray ray_p2x(fixed_p, ws);

    //由p点射出的光线相交的距离小于光线的距离，说明被物体遮挡，需要计算直接光照
    if ((intersect(ray_p2x).distance - distance > -0.0001))
    {
        L_dir = emit * intersection.m->eval(wo, ws, N) * dotProduct(ws, N) * dotProduct(-ws, NN) / (distance * distance) / pdf;
    }

    //俄罗斯轮盘赌
    if (get_random_float() < RussianRoulette)
    {
        //计算间接光照
        Vector3f wi = intersection.m->sample(wo, N).normalized();
        Ray nextRay(fixed_p, wi);
        Intersection nextInter = intersect(nextRay);
        if (nextInter.happened && !nextInter.m->hasEmission())
        {
            L_indir = castRay(nextRay, depth + 1) * intersection.m->eval(wo, wi, N) * dotProduct(wi, N) / intersection.m->pdf(wo, wi, N) / RussianRoulette;
        }
    }

    return L_dir + L_indir;
}
```

> 关于p点的问题：https://zhuanlan.zhihu.com/p/606074595

需要注意的是：

- 从p点向x点射出光线，需要对p点进行修正，否则会出现不正确的阴影
- 直接光照的判断条件中，需要将eps调大，否则会出现黑边



下面是spp较小的结果图：

![image-20230924113111684](F:\Typora_Image\image-20230924113111684.png)



另外，**p点要修正**，否则会产生以下的错误结果：

![image-20230924120408602](F:\Typora_Image\image-20230924120408602.png)



#### 随机数问题

> 随机数构造器优化：https://zhuanlan.zhihu.com/p/606074595
>
> C++的性能分析器：https://blog.csdn.net/u011942101/article/details/123656944

在`global.hpp`中将以下三个变量设置为`static`，能够加速图像的生成。由于这个函数频繁地被调用，其中的变量经常被反复构建，造成了很大性能开销。将这些变量设置为static，那么这些变量只会初始化一次，避免了重复构建。

```c++
inline float get_random_float()
{
    static std::random_device dev;
    static std::mt19937 rng(dev());
    static std::uniform_real_distribution<float> dist(0.f, 1.f); // distribution in range [1, 6]

    return dist(rng);
}
```



#### 多线程

> C++多线程教程：https://blog.csdn.net/sjc_0910/article/details/118861539

以下是实现多线程需要注意的地方：

- 可以将渲染像素的过程使用lambda表达式写出来，这样能够方便地使用多线程。
- `UpdateProgress`这个函数可能会造成冲突，需要上锁

```c++
void Renderer::Render(const Scene& scene)
{
    std::vector<Vector3f> framebuffer(scene.width * scene.height);

    float scale = tan(deg2rad(scene.fov * 0.5));
    float imageAspectRatio = scene.width / (float)scene.height;
    Vector3f eye_pos(278, 273, -800);

    //开启多线程
    int numThreads = std::thread::hardware_concurrency();
    std::vector<std::thread> threads(numThreads);
    std::mutex mutex;
    int process = 0;
    int rowsPerThread = scene.height / numThreads;

    // change the spp value to change sample ammount
    int spp = 16;
    std::cout << "SPP: " << spp << "\n";

    //将渲染每个像素的过程封装到一个函数中，其中捕获列表捕获当前作用域内所有变量，引用传递
    auto renderPixel = [&](uint32_t startRow, uint32_t endRow) {
        //m是像素数组的下标，由于多线程是随机的，因此需要在每次运行前判断当前的m
        int m = startRow * scene.width;
        for (uint32_t j = startRow; j < endRow; ++j) {
            for (uint32_t i = 0; i < scene.width; ++i) {
                // generate primary ray direction
                float x = (2 * (i + 0.5) / (float)scene.width - 1) *
                    imageAspectRatio * scale;
                float y = (1 - 2 * (j + 0.5) / (float)scene.height) * scale;

                Vector3f dir = normalize(Vector3f(-x, y, 1));
                for (int k = 0; k < spp; k++) {
                    framebuffer[m] += scene.castRay(Ray(eye_pos, dir), 0) / spp;
                }
                m++;
            }
            //需要对这个操作进行上锁，防止冲突
            mutex.lock();
            UpdateProgress(++process / (float)scene.height);
            mutex.unlock();
        }
    };

    //使用多线程执行renderPixel函数
    for (int t = 0; t < numThreads; ++t) {
        threads[t] = std::thread(renderPixel, t * rowsPerThread, (t + 1) * rowsPerThread);
    }

    // 等待所有线程完成
    for (int i = 0; i < numThreads; ++i) {
        threads[i].join();
    }  

    UpdateProgress(1.f);

    // save framebuffer to file
    FILE* fp = fopen("binary.ppm", "wb");
    (void)fprintf(fp, "P6\n%d %d\n255\n", scene.width, scene.height);
    for (auto i = 0; i < scene.height * scene.width; ++i) {
        static unsigned char color[3];
        color[0] = (unsigned char)(255 * std::pow(clamp(0, 1, framebuffer[i].x), 0.6f));
        color[1] = (unsigned char)(255 * std::pow(clamp(0, 1, framebuffer[i].y), 0.6f));
        color[2] = (unsigned char)(255 * std::pow(clamp(0, 1, framebuffer[i].z), 0.6f));
        fwrite(color, 1, 3, fp);
    }
    fclose(fp);    
}
```

 效果提升显著，从18分钟缩减到1分钟。



#### 问题

- 渲染结果较暗，出现横向黑色条纹的情况，很可能是因为直接光部分由于精度问题，被错误判断为遮挡，请试着通过精度调整放宽相交限制。

  **解决方法：**增大直接光照判断条件中的eps。

- 渲染结果全黑或几乎全黑，右边完全看不见物体，是因为包围盒AABB的判断错误，天花板和周围的面是没有厚度的，这导致在判断的时候没有纳入相交。

  **解决方法：**需要考虑`t_enter == t_exit`的情况



最终结果如图：

![image-20231006160423794](F:\Typora_Image\image-20231009221232410.png)

