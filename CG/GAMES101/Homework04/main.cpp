#include <chrono>
#include <iostream>
#include <opencv2/opencv.hpp>

std::vector<cv::Point2f> control_points;

void mouse_handler(int event, int x, int y, int flags, void *userdata) 
{
    if (event == cv::EVENT_LBUTTONDOWN && control_points.size() < 4) 
    {
        std::cout << "Left button of the mouse is clicked - position (" << x << ", "
        << y << ")" << '\n';
        control_points.emplace_back(x, y);
    }     
}

void naive_bezier(const std::vector<cv::Point2f> &points, cv::Mat &window) 
{
    auto &p_0 = points[0];
    auto &p_1 = points[1];
    auto &p_2 = points[2];
    auto &p_3 = points[3];

    for (double t = 0.0; t <= 1.0; t += 0.001) 
    {
        auto point = std::pow(1 - t, 3) * p_0 + 3 * t * std::pow(1 - t, 2) * p_1 +
                 3 * std::pow(t, 2) * (1 - t) * p_2 + std::pow(t, 3) * p_3;

        window.at<cv::Vec3b>(point.y, point.x)[2] = 255;
    }
}

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

int main() 
{
    cv::Mat window = cv::Mat(700, 700, CV_8UC3, cv::Scalar(0));
    cv::cvtColor(window, window, cv::COLOR_BGR2RGB);
    cv::namedWindow("Bezier Curve", cv::WINDOW_AUTOSIZE);

    cv::setMouseCallback("Bezier Curve", mouse_handler, nullptr);

    int key = -1;
    while (key != 27) 
    {
        for (auto &point : control_points) 
        {
            cv::circle(window, point, 3, {255, 255, 255}, 3);
        }

        if (control_points.size() == 4) 
        {
            //naive_bezier(control_points, window);
            bezier(control_points, window);

            cv::imshow("Bezier Curve", window);
            cv::imwrite("my_bezier_curve.png", window);
            key = cv::waitKey(0);

            return 0;
        }

        cv::imshow("Bezier Curve", window);
        key = cv::waitKey(20);
    }

return 0;
}
