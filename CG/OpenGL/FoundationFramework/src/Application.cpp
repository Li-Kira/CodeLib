#include <GL/glew.h>
#include <GLFW/glfw3.h>

#include <iostream>
#include <fstream>
#include <string>
#include <sstream>

#include "Renderer.h"
#include "VertexArray.h"
#include "VertexBuffer.h"
#include "VertexBufferLayout.h"
#include "IndexBuffer.h"
#include "Shader.h"
#include "Texture.h"

#include "glm/glm.hpp"
#include "glm/gtc/matrix_transform.hpp"

#include "imgui/imgui.h"
#include "imgui/imgui_impl_glfw.h"
#include "imgui/imgui_impl_opengl3.h"


int main(void)
{
    GLFWwindow* window;

    /* Initialize the library */
    if (!glfwInit())
        return -1;

    /* Create a windowed mode window and its OpenGL context */
    //window = glfwCreateWindow(640, 480, "Hello World", NULL, NULL);
    window = glfwCreateWindow(960, 540, "Hello World", NULL, NULL);
    if (!window)
    {
        glfwTerminate();
        return -1;
    }

    /* Make the window's context current */
    glfwMakeContextCurrent(window);

    //将刷新率设置为1
    glfwSwapInterval(1);

    if (glewInit() != GLEW_OK)
        std::cout << "Error" << std::endl;

    std::cout << glGetString(GL_VERSION) << std::endl;


    // Vertex Info
    float positions[] = {
         -50.0f, -50.0f, 0.0f, 0.0f,//0
          50.0f, -50.0f, 1.0f, 0.0f,//1
          50.0f,  50.0f, 1.0f, 1.0f,//2
         -50.0f,  50.0f, 0.0f, 1.0f,//3
    };

    unsigned int indices[] = {
        0, 1, 2,
        2, 3, 0
    };

    GLCall(glEnable(GL_BLEND));
    GLCall(glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA));

    //unsigned int vao;
    //GLCall(glGenVertexArrays(1, &vao));
    //GLCall(glBindVertexArray(vao));

    VertexArray va;
    VertexBuffer vb(positions, 4 * 4 * sizeof(float));

    VertexBufferLayout layout;
    layout.Push<float>(2);
    layout.Push<float>(2);
    va.AddBuffer(vb, layout);

    IndexBuffer ib(indices, 6);

    glm::mat4 proj = glm::ortho(0.0f, 960.0f, 0.0f, 540.0f, -1.0f, 1.0f);
    glm::mat4 view = glm::translate(glm::mat4(1.0f), glm::vec3(0, 0, 0));
    glm::mat4 mocdel = glm::translate(glm::mat4(1.0f), glm::vec3(200, 200, 0));
    glm::mat4 mvp = proj * view * mocdel;
    glm::vec3 translationA(200, 0, 0);
    glm::vec3 translationB(400, 0, 0);

    Shader shader("res/shader/Basic.shader");
    shader.Bind();
    shader.SetUniform4f("u_Color", 0.8f, 0.3f, 0.8f, 1.0f);
    shader.SetUniformMat4f("u_MVP", mvp);

    Texture texture("res/textures/1500981_cg_scale.png");
    texture.Bind();
    //Texture Bind slot = 0， 所以这里也是0
    shader.SetUniform1i("u_Texture", 0);

	ib.UnBind();
    vb.UnBind();
    va.UnBind();
    shader.UnBind();

    Renderer renderer;

    // ImGui Setup
	IMGUI_CHECKVERSION();
	ImGui::CreateContext();
	ImGuiIO& io = ImGui::GetIO(); (void)io;
	io.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;     // Enable Keyboard Controls
	io.ConfigFlags |= ImGuiConfigFlags_NavEnableGamepad;      // Enable Gamepad Controls

    ImGui::StyleColorsDark();

    ImGui_ImplGlfw_InitForOpenGL(window, true);
    ImGui_ImplOpenGL3_Init("#version 440");
    
    // SetUniform 参数
    float r = 0.0f;
    float increase = 0.05f;

    /* Loop until the user closes the window */
    while (!glfwWindowShouldClose(window))
    {
        /* Render here */
        renderer.Clear();

        // Start the Dear ImGui frame
		ImGui_ImplOpenGL3_NewFrame();
		ImGui_ImplGlfw_NewFrame();
		ImGui::NewFrame();

		//为了使用SetUniform4f，仍需要Bind
        {
            glm::mat4 mocdel = glm::translate(glm::mat4(1.0f), translationA);
			glm::mat4 mvp = proj * view * mocdel;
            shader.Bind();
            shader.SetUniformMat4f("u_MVP", mvp);
            
            renderer.Draw(va, ib, shader);
        }
		
        {
            glm::mat4 mocdel = glm::translate(glm::mat4(1.0f), translationB);
            glm::mat4 mvp = proj * view * mocdel;
            shader.Bind();
            shader.SetUniformMat4f("u_MVP", mvp);
            
            renderer.Draw(va, ib, shader);
        }

        if (r > 1.0f)
            increase = -0.05f;
        else if (r < 0.0f)
            increase = 0.05f;

        r += increase;

        // ImGui Content
		{
            ImGui::Begin("Hello, world!");
            ImGui::SliderFloat3("TranslationA", &translationA.x, 0.0f, 960.0f);
            ImGui::SliderFloat3("TranslationB", &translationB.x, 0.0f, 960.0f);
			ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / io.Framerate, io.Framerate);
			ImGui::End();
		}

        // ImGui Rendering
		ImGui::Render();
		ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());

        /* Swap front and back buffers */
        glfwSwapBuffers(window);

        /* Poll for and process events */
        glfwPollEvents();
    }


	ImGui_ImplOpenGL3_Shutdown();
	ImGui_ImplGlfw_Shutdown();
	ImGui::DestroyContext();

    glfwTerminate();
    
    return 0;
}