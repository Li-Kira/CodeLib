//
// Created by LEI XU on 4/27/19.
//

#ifndef RASTERIZER_TEXTURE_H
#define RASTERIZER_TEXTURE_H
#include "global.hpp"
#include <eigen3/Eigen/Eigen>
#include <opencv2/opencv.hpp>
class Texture{
private:
    cv::Mat image_data;

public:
    Texture(const std::string& name)
    {
        image_data = cv::imread(name);
        cv::cvtColor(image_data, image_data, cv::COLOR_RGB2BGR);
        width = image_data.cols;
        height = image_data.rows;
    }

    int width, height;

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


};
#endif //RASTERIZER_TEXTURE_H
