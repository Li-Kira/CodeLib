# GAMES101作业1 - 变换

模型矩阵：

```c++
Eigen::Matrix4f get_model_matrix(float rotation_angle)
{
    Eigen::Matrix4f model = Eigen::Matrix4f::Identity();

    // TODO: Implement this function
    // Create the model matrix for rotating the triangle around the Z axis.
    // Then return it.

    float rotation_radians = rotation_angle * MY_PI / 180.0;
    float cos_value = cos(rotation_radians);
    float sin_value = sin(rotation_radians);

    model << 
        cos_value, -sin_value, 0, 0, 
        sin_value, cos_value, 0, 0, 
        0, 0, 1, 0, 
        0, 0, 0, 1;

    return model;
}
```



**修改之后的投影矩阵：**

```c++
Eigen::Matrix4f get_projection_matrix(float eye_fov, float aspect_ratio, float zNear, float zFar)
{

    Eigen::Matrix4f projection = Eigen::Matrix4f::Identity();
    
    float fov = eye_fov * MY_PI / 180.0f;
    // zNear is negative, need * -1.
    float top = -zNear * tan(fov * 0.5);
    Eigen::Matrix4f M_ortho;
    Eigen::Matrix4f M_persp2ortho;

    M_ortho <<
        1 / (aspect_ratio * top), 0, 0, 0,
        0, 1 / top, 0, 0,
        0, 0, -2 / zFar - zNear, -(zFar + zNear) / (zFar - zNear),
        0, 0, 0, 1;

    M_persp2ortho <<
        zNear, 0, 0, 0,
        0, zNear, 0, 0,
        0, 0, zNear + zFar, -zNear * zFar,
        0, 0, 1, 0;

    projection = M_ortho * M_persp2ortho;

    return projection;
}
```



**旋转矩阵：**

```c++
Eigen::Matrix4f get_rotation(Eigen::Vector3f axis, float angle)
{
    Eigen::Matrix4f model = Eigen::Matrix4f::Identity();

    float rotation_radians = angle * MY_PI / 180.0;
    float cos_value = cos(rotation_radians);
    float sin_value = sin(rotation_radians);

    Eigen::Matrix4f I = Eigen::Matrix4f::Identity();

    Eigen::Matrix4f M_axis;
    M_axis << axis.x(), axis.y(), axis.z(), 0;
    Eigen::MatrixXf M_axis_transposed = M_axis.transpose();

    Eigen::Matrix4f N;
    N << 0, -M_axis.z(), M_axis.y(), 0,
        M_axis.z(), 0, -M_axis.x(), 0,
        -M_axis.y(), M_axis.x(), 0, 0,
        0, 0, 0, 1;

    model = cos_value * I + (1 - cos_value) * M_axis * M_axis_transposed + sin_value * N;

    return model;
}
```

