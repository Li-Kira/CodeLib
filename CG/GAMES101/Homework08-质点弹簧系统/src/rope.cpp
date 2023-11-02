#include <iostream>
#include <vector>

#include "CGL/vector2D.h"

#include "mass.h"
#include "rope.h"
#include "spring.h"

namespace CGL {

    Rope::Rope(Vector2D start, Vector2D end, int num_nodes, float node_mass, float k, vector<int> pinned_nodes)
    {
        // TODO (Part 1): Create a rope starting at `start`, ending at `end`, and containing `num_nodes` nodes.

//        Comment-in this part when you implement the constructor
//        for (auto &i : pinned_nodes) {
//            masses[i]->pinned = true;
//        }

    for (int i = 0; i < num_nodes; i++)
    {
        Vector2D pos = start + (end - start) * ((double)i / ((double)num_nodes - 1.0));
        masses.push_back(new Mass(pos, node_mass, false));
        
    }

    for (int i = 0; i < num_nodes - 1; i++)
    {
        springs.push_back(new Spring(masses[i], masses[i+1], k));
    }

    for (auto &i : pinned_nodes) {
            masses[i]->pinned = true;
        }
        
    }

    void Rope::simulateEuler(float delta_t, Vector2D gravity)
    {
        for (auto &s : springs)
        {
            // TODO (Part 2): Use Hooke's law to calculate the force on a node
            float l = (s->m1->position - s->m2->position).norm();
            s->m1->forces += -(s->k) * (s->m1->position - s->m2->position) / l * (l - s->rest_length);
            s->m2->forces += -(s->k) * (s->m2->position - s->m1->position) / l * (l - s->rest_length);
        }

        for (auto &m : masses)
        {
            if (!m->pinned)
            {
                // TODO (Part 2): Add the force due to gravity, then compute the new velocity and position
                //Vector2D a = m->forces / m->mass + gravity;
                //m->velocity += a * delta_t; 
                //m->position += m->velocity * delta_t; 
                

                // TODO (Part 2): Add global damping
                float kd = 0.05;
                Vector2D a = m->forces / m->mass + gravity - kd * m->velocity / m->mass;
                m->velocity += a * delta_t; 
                m->position += m->velocity * delta_t;
            }

            // Reset all forces on each mass
            m->forces = Vector2D(0, 0);
        }
    }

    void Rope::simulateVerlet(float delta_t, Vector2D gravity)
    {
        for (auto &s : springs)
        {
            // TODO (Part 3): Simulate one timestep of the rope using explicit Verlet （solving constraints)
            float l = (s->m1->position - s->m2->position).norm();
            s->m1->forces += -(s->k) * (s->m1->position - s->m2->position) / l * (l - s->rest_length);
            s->m2->forces += -(s->k) * (s->m2->position - s->m1->position) / l * (l - s->rest_length);

        }

        for (auto &m : masses)
        {
            if (!m->pinned)
            {
                Vector2D temp_position = m->position;
                // TODO (Part 3.1): Set the new position of the rope mass
                //Vector2D a = m->forces / m->mass + gravity;
                //m->position =  temp_position + (temp_position - m->last_position) + a * delta_t * delta_t;
                //m->last_position = temp_position;

                // TODO (Part 4): Add global Verlet damping
                Vector2D a = m->forces / m->mass + gravity;
                float damping_factor = 0.00005;
                m->position = temp_position + (1 - damping_factor) * (temp_position - m->last_position) + a * delta_t * delta_t;
                m->last_position = temp_position;
                
            }
            // Reset all forces on each mass
            m->forces = Vector2D(0, 0);
        }
    }
}
