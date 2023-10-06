#pragma once

class IndexBuffer
{
private:
	unsigned int m_RendererID;
	unsigned int m_Count;
public:
	//������ģ�����������������ʹ��unsigned int
	IndexBuffer(const unsigned int* data, unsigned int count);
	~IndexBuffer();

	//����û���޸��κβ���������֮����const�������
	void Bind() const;
	void UnBind() const;

	inline unsigned int GetCount() const { return m_Count; }
};