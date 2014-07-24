#pragma once

#include <vector>
#include <string>
#include <boost/shared_ptr.hpp>

#include <boost/typeof/std/utility.hpp>
#include <iostream>
#include <boost/make_shared.hpp>
#include <typeinfo>
#include <boost/mpl/if.hpp>
#include <boost/noncopyable.hpp>

namespace TowerLua
{
	template<class Src>
	const void* void_cast(Src src)
	{
		union
		{
			void* d;
			Src s;
		} convertor;
		convertor.d = nullptr;
		convertor.s = src;
		return (convertor.d);
	}

	class HelpContainer : private boost::noncopyable
	{
		private:
			static std::string m_buffer;
			HelpContainer()	{}
		public:
			static bool enabled;

			static inline std::string & GetString()
			{
				return m_buffer;
			}

			static inline void ClearString()
			{
				m_buffer.clear();
			}

			static void AddInfo(const char* type, const char* name, const void *ptr=nullptr, const char* ex1 = nullptr, const void *ptr2 = nullptr, const char* ex2 = nullptr)
			{
				std::string tmp;
				tmp+=type;
				tmp+="|";
				tmp+=name;
				tmp+="|";

				if (ex1!=nullptr)
				{
					char t[20]={0};
					sprintf_s(t,"%p", ptr);
					tmp+=t;
					tmp+="|";
					tmp+=ex1;
					tmp+="|";
				}
				if (ex2!=nullptr)
				{
					char t[20]={0};
					sprintf_s(t,"%p", ptr2);
					tmp+=t;
					tmp+="|";
					tmp+=ex2;
					tmp+="|";
				}
				tmp+="\n";
				m_buffer+=tmp;
			}
	};

	class HelpNavigator : private boost::noncopyable
	{
		private:
			static std::string *m_string;
		public:
			HelpNavigator(const char* type, const char* symbol, const char* fileName, unsigned int line, const void* pointer)
			{
				if (nullptr == m_string)
					m_string = new std::string();
				(*m_string)+=type;
				(*m_string)+="|";
				(*m_string)+=symbol;
				(*m_string)+="|";
				(*m_string)+=fileName;
				(*m_string)+="|";
				char t[30]={0};
				sprintf_s(t,"%u|%p\n", line, pointer);
				(*m_string)+=t;
			}

			HelpNavigator(const char* type, const char* symbol, unsigned int index, const char* types, const char* fileName, unsigned int line, const void* pointer)
			{
				if (nullptr == m_string)
					m_string = new std::string();
				(*m_string)+=type;
				(*m_string)+="|";
				(*m_string)+=symbol;
				char t2[30]={0};
				sprintf_s(t2," %u",index);
				(*m_string)+=t2;
				(*m_string)+="|";
				(*m_string)+=types;
				(*m_string)+="|";
				(*m_string)+=fileName;
				(*m_string)+="|";
				char t[30]={0};
				sprintf_s(t,"%u|%p\n", line, pointer);
				(*m_string)+=t;
			}

			static inline std::string& GetString()
			{
				if (nullptr == m_string)
					m_string = new std::string();
				return (*m_string);
			}
	};

#ifdef LOG_FILE
	#define TowerLua__JOIN2(x,y) x##y
	#define TowerLua__JOIN(x,y) TowerLua__JOIN2(x,y)
	#define __HELP_MARK(type, symbol, ptr) static TowerLua::HelpNavigator TowerLua__JOIN(g_helpMarker,__COUNTER__)(type, symbol, __FILE__, __LINE__, TowerLua::void_cast(ptr));
	#define __HELP_MARK2(type, symbol) static TowerLua::HelpNavigator TowerLua__JOIN(g_helpMarker,__COUNTER__)(type, symbol, __FILE__, __LINE__, nullptr);
	
	#define HELP_CLASS(type)					__HELP_MARK2("CLASS", typeid( type ).name())
	#define HELP_METHOD(ptr)					__HELP_MARK("METHOD", typeid(ptr).name(), ptr)
	#define HELP_CONST(ptr)						__HELP_MARK("CONST", typeid(ptr).name(), ptr)
	#define HELP_VAR(ptr)						__HELP_MARK("VAR", typeid(ptr).name(), ptr)
	#define HELP_FUNCTION(ptr)					__HELP_MARK("FUNCTION", typeid(ptr).name(), ptr)
	#define HELP_PROPERTY(ptr)					__HELP_MARK("PROPERTY", typeid(ptr).name(), ptr)
	#define HELP_EVENT(ptr)						__HELP_MARK("EVENT", typeid(ptr).name(), ptr)
	#define HELP_CONSTRUCTOR(_class, number)	__HELP_MARK("CONSTRUCTOR", typeid(_class).name(), number)
	#define HELP_MODULE(fullName)				__HELP_MARK2("MODULE", fullName)
	#define HELP_NAMESPACE(fullName)			__HELP_MARK2("NAMESPACE", fullName)
	#define HELP_PACKAGE(fullName)				__HELP_MARK2("PACKAGE", fullName)
	#define HELP_INFO(fullName)					__HELP_MARK2("INFO", fullName)
	#define HELP_ENUM(type)						__HELP_MARK2("ENUM", typeid(type).name())
	#define HELP_ENUM_ITEM(value)				__HELP_MARK("ENUM_ITEM", typeid(value).name(), value)
	#define HELP_BASE(type)						__HELP_MARK2("BASE", typeid(type).name())
#else
	#define __HELP_MARK(type, symbol, ptr)
	#define __HELP_MARK2(type, symbol)
	#define HELP_CLASS(type)  
	#define HELP_METHOD(ptr)	 
	#define HELP_CONST(ptr)				 	
	#define HELP_VAR(ptr)				  	
	#define HELP_FUNCTION(ptr)			 	
	#define HELP_PROPERTY(ptr)				 
	#define HELP_EVENT(ptr)  
	#define HELP_CONSTRUCTOR(_class, number) 
	#define HELP_MODULE(fullName)			 
	#define HELP_NAMESPACE(fullName)	 
	#define HELP_PACKAGE(fullName) 
	#define HELP_INFO(fullName) 
	#define HELP_ENUM(type)				  	
	#define HELP_ENUM_ITEM(value)		 		
	#define HELP_BASE(type)				 	
#endif

}