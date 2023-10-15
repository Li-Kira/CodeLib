//
// Created by Göksu Güvendiren on 2019-05-14.
//

#include "Scene.hpp"


void Scene::buildBVH() {
    printf(" - Generating BVH...\n\n");
    this->bvh = new BVHAccel(objects, 1, BVHAccel::SplitMethod::NAIVE);
}

Intersection Scene::intersect(const Ray &ray) const
{
    return this->bvh->Intersect(ray);
}

void Scene::sampleLight(Intersection &pos, float &pdf) const
{
    float emit_area_sum = 0;
    for (uint32_t k = 0; k < objects.size(); ++k) {
        if (objects[k]->hasEmit()){
            emit_area_sum += objects[k]->getArea();
        }
    }
    float p = get_random_float() * emit_area_sum;
    emit_area_sum = 0;
    for (uint32_t k = 0; k < objects.size(); ++k) {
        if (objects[k]->hasEmit()){
            emit_area_sum += objects[k]->getArea();
            if (p <= emit_area_sum){
                objects[k]->Sample(pos, pdf);
                break;
            }
        }
    }
}

bool Scene::trace(
        const Ray &ray,
        const std::vector<Object*> &objects,
        float &tNear, uint32_t &index, Object **hitObject)
{
    *hitObject = nullptr;
    for (uint32_t k = 0; k < objects.size(); ++k) {
        float tNearK = kInfinity;
        uint32_t indexK;
        Vector2f uvK;
        if (objects[k]->intersect(ray, tNearK, indexK) && tNearK < tNear) {
            *hitObject = objects[k];
            tNear = tNearK;
            index = indexK;
        }
    }


    return (*hitObject != nullptr);
}

// Implementation of Path Tracing
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
    //原eps为0.0001,适当提高可以减少黑色横纹
    if ((intersect(ray_p2x).distance - distance > -0.0016f))
    {
        if(pdf > EPSILON)
            L_dir = emit * intersection.m->eval(wo, ws, N) * dotProduct(ws, N) * dotProduct(-ws, NN) / (distance * distance) / pdf;
    }

    //俄罗斯轮盘赌
    if (get_random_float() < RussianRoulette)
    {
        //计算间接光照
        Vector3f wi = intersection.m->sample(wo, N).normalized();
        Ray nextRay(fixed_p, wi);
        Intersection nextInter = intersect(nextRay);

        //防止pdf趋近0的情况
        if (intersection.m->pdf(wo, wi, N) > EPSILON)
        {
			if (nextInter.happened && !nextInter.m->hasEmission())
			{
				L_indir = castRay(nextRay, depth + 1) * intersection.m->eval(wo, wi, N) * dotProduct(wi, N) / intersection.m->pdf(wo, wi, N) / RussianRoulette;
			}
        }
        
    }


    //解决白点过多的问题
    Vector3f color = L_dir + L_indir;
    color.x = clamp(0, 1, color.x);
    color.y = clamp(0, 1, color.y);
    color.z = clamp(0, 1, color.z);

    return color;
}
