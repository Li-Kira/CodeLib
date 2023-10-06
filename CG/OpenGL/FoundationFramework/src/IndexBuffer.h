#pragma once

class IndexBuffer
{
private:
	unsigned int m_RendererID;
	unsigned int m_Count;
public:
	//如果你的模型三角面索引不大就使用unsigned int
	IndexBuffer(const unsigned int* data, unsigned int count);
	~IndexBuffer();

	//他们没有修改任何参数，所有之后用const对象调用
	void Bind() const;
	void UnBind() const;

	inline unsigned int GetCount() const { return m_Count; }
};