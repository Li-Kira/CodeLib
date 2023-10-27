# GAMES101作业4 - 贝塞尔曲线

> 代码地址：
>
> https://github.com/Li-Kira/CodeLib/tree/main/CG/GAMES101/Homework04-%E8%B4%9D%E5%A1%9E%E5%B0%94%E6%9B%B2%E7%BA%BF



## 目标

- 实现De Casteljau 算法
- 实现对 Bézier 曲线的反走样。(对于一个曲线上的点，不只把它对应于一个像 素，你需要根据到像素中心的距离来考虑与它相邻的像素的颜色)



## De Casteljau 算法

对于三次贝塞尔曲线（四个控制点）使用以下公式计算每个t时刻的点的位置。
$$
b^n(t) = b_0 (1-t)^3 + b_1 3t(1-t)^2 + b_2 3t^2(1-t) + b_3 t^3
$$


```c++
cv::Point2f recursive_bezier(const std::vector<cv::Point2f> &control_points, float t) 
{
    // TODO: Implement de Casteljau's algorithm
    auto b0 = control_points[0];
    auto b1 = control_points[1];
    auto b2 = control_points[2];
    auto b3 = control_points[3];

    auto bn = b0 * pow((1 - t), 3) + b1 * 3 * t * pow((1 - t), 2) + b2 * 3 * pow(t, 2) * (1 - t) + b3 * pow(t, 3);
    return bn;
}

void bezier(const std::vector<cv::Point2f> &control_points, cv::Mat &window) 
{
    // TODO: Iterate through all t = 0 to t = 1 with small steps, and call de Casteljau's 
    // recursive Bezier algorithm.

    std::vector<cv::Point2f> Texel(9);

    for (double t = 0.0; t <= 1.0; t += 0.001)
    {
        auto point = recursive_bezier(control_points, t);
        window.at<cv::Vec3b>(point.y, point.x)[1] = 255;
    }
}
```



![image-20230829132335330](F:\Typora_Image\image-20230829132335330.png)



## 抗锯齿

> 思路：https://blog.csdn.net/ycrsw/article/details/124117190

取该点周围的3x3的像素的中心点到t时刻点的距离。
$$
ratio = 1 - \frac{\sqrt{2}}{3} * distance
$$

$$
color = 255 * ratio
$$

重复的点取该点上的最大值。

```c++
void bezier(const std::vector<cv::Point2f> &control_points, cv::Mat &window) 
{
    // TODO: Iterate through all t = 0 to t = 1 with small steps, and call de Casteljau's 
    // recursive Bezier algorithm.

    std::vector<cv::Point2f> Texel(9);

    for (double t = 0.0; t <= 1.0; t += 0.001)
    {
        auto point = recursive_bezier(control_points, t);

        Texel[0] = cv::Point2f(std::floor(point.x) - 0.5, std::floor(point.y) + 1 + 0.5);
        Texel[1] = cv::Point2f(std::floor(point.x) + 0.5, std::floor(point.y) + 1 + 0.5);
        Texel[2] = cv::Point2f(std::floor(point.x) + 1 + 0.5, std::floor(point.y) + 1 + 0.5);
        Texel[3] = cv::Point2f(std::floor(point.x) - 0.5, std::floor(point.y) + 0.5);
        Texel[4] = cv::Point2f(std::floor(point.x) + 0.5, std::floor(point.y) + 0.5);
        Texel[5] = cv::Point2f(std::floor(point.x) + 1 + 0.5, std::floor(point.y) + 0.5);
        Texel[6] = cv::Point2f(std::floor(point.x) - 0.5, std::floor(point.y) - 0.5);
        Texel[7] = cv::Point2f(std::floor(point.x) + 0.5, std::floor(point.y) - 0.5);
        Texel[8] = cv::Point2f(std::floor(point.x) + 1 + 0.5, std::floor(point.y) - 0.5);


        for (int i = 0; i < Texel.size(); i++)
        {
            float distance = sqrt(pow((Texel[i].x - point.x), 2) + pow((Texel[i].y - point.y), 2));
            float Ratio = 1 - sqrt(2) * distance / 3.0;
            window.at<cv::Vec3b>(Texel[i].y, Texel[i].x)[1] = std::fmax(window.at<cv::Vec3b>(Texel[i].y, Texel[i].x)[1], 255 * Ratio);
        }

        //window.at<cv::Vec3b>(point.y, point.x)[1] = 255;
    }

}
```

![作业4](F:\Typora_Image\作业4.png)



