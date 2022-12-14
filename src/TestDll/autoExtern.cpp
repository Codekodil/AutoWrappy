#include"PointerDelete.h"
#include"PointerDispose.h"
#include"SelfPointer.h"
#include"SharedAlive.h"
#include"SharedAll.h"
#include<memory>
#include<vector>
#include<string>
#include<glm/glm.hpp>
extern "C"{
__declspec(dllexport) void* __stdcall Wrappy_Shared_Ptr_Get(std::shared_ptr<void>* self){return self->get();}
__declspec(dllexport) TestDll::DisposeBase* __stdcall Wrappy_New_DisposeBase(){auto inner_result=new TestDll::DisposeBase();return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_DisposeBase_Ten(TestDll::DisposeBase* self){auto inner_result=self->Ten();return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_Delete_DisposeBase(TestDll::DisposeBase* self){delete self;}
__declspec(dllexport) TestDll::PointerDelete* __stdcall Wrappy_New_PointerDelete(){auto inner_result=new TestDll::PointerDelete();return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_PointerDelete_Nothing(TestDll::PointerDelete* self){self->Nothing();}
__declspec(dllexport) int __stdcall Wrappy_PointerDelete_Nine(TestDll::PointerDelete* self){auto inner_result=self->Nine();return inner_result;}
__declspec(dllexport) float __stdcall Wrappy_PointerDelete_Half(TestDll::PointerDelete* self,float arg_a){auto inner_result=self->Half(arg_a);return inner_result;}
__declspec(dllexport) double __stdcall Wrappy_PointerDelete_Add(TestDll::PointerDelete* self,int arg_l,double arg_r){auto inner_result=self->Add(arg_l,arg_r);return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_PointerDelete_Sum(TestDll::PointerDelete* self,int* p_arg_n,int l_arg_n){std::span<int> span_arg_n(p_arg_n,l_arg_n);auto inner_result=self->Sum(span_arg_n);return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_Delete_PointerDelete(TestDll::PointerDelete* self){delete self;}
__declspec(dllexport) TestDll::PointerDispose* __stdcall Wrappy_New_PointerDispose(){auto inner_result=new TestDll::PointerDispose();return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_PointerDispose_Five(TestDll::PointerDispose* self){auto inner_result=self->Five();return inner_result;}
__declspec(dllexport) void* __stdcall Wrappy_PointerDispose_ThisPointer(TestDll::PointerDispose* self){auto inner_result=self->ThisPointer();return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_PointerDispose_PointerValue(TestDll::PointerDispose* self,void* arg_pointer){auto inner_result=self->PointerValue(arg_pointer);return inner_result;}
__declspec(dllexport) float __stdcall Wrappy_PointerDispose_Sum(TestDll::PointerDispose* self,glm::vec3* arg_vec){auto inner_result=self->Sum(*arg_vec);return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_PointerDispose_Normalice(TestDll::PointerDispose* self,glm::vec2* p_arg_vecs,int l_arg_vecs){std::span<glm::vec2> span_arg_vecs(p_arg_vecs,l_arg_vecs);self->Normalice(span_arg_vecs);}
__declspec(dllexport) void __stdcall Wrappy_PointerDispose_Transform(TestDll::PointerDispose* self,glm::vec4* p_arg_vecs,int l_arg_vecs,glm::mat4* arg_transform){std::span<glm::vec4> span_arg_vecs(p_arg_vecs,l_arg_vecs);self->Transform(span_arg_vecs,*arg_transform);}
__declspec(dllexport) void __stdcall Wrappy_PointerDispose_Rotate(TestDll::PointerDispose* self,glm::vec3* p_arg_points,int l_arg_points,glm::quat* arg_rotation){std::span<glm::vec3> span_arg_points(p_arg_points,l_arg_points);self->Rotate(span_arg_points,*arg_rotation);}
__declspec(dllexport) void __stdcall Wrappy_PointerDispose_SetEvent_OnFive(TestDll::PointerDispose* self, void(__stdcall* event)()){self->OnFive = event;}
__declspec(dllexport) void __stdcall Wrappy_Delete_PointerDispose(TestDll::PointerDispose* self){delete self;}
__declspec(dllexport) std::shared_ptr<TestDll::SelfPointer>* __stdcall Wrappy_New_SelfPointer(){auto inner_result=new std::shared_ptr<TestDll::SelfPointer>(new TestDll::SelfPointer());return inner_result;}
__declspec(dllexport) void* __stdcall Wrappy_SelfPointer_GetThis(std::shared_ptr<TestDll::SelfPointer>* self){auto inner_result=(*self)->GetThis();return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_Delete_SelfPointer(std::shared_ptr<TestDll::SelfPointer>* self){delete self;}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAlive>* __stdcall Wrappy_New_SharedAlive(double arg_a){auto inner_result=new std::shared_ptr<TestDll::SharedAlive>(new TestDll::SharedAlive(arg_a));return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_SharedAlive_Two(std::shared_ptr<TestDll::SharedAlive>* self){auto inner_result=(*self)->Two();return inner_result;}
__declspec(dllexport) TestDll::PointerDispose* __stdcall Wrappy_SharedAlive_MakeDispose(std::shared_ptr<TestDll::SharedAlive>* self){auto inner_result=(*self)->MakeDispose();return inner_result;}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAll>* __stdcall Wrappy_SharedAlive_MakePrint(std::shared_ptr<TestDll::SharedAlive>* self,int arg_i){auto inner_result=new std::shared_ptr<TestDll::SharedAll>((*self)->MakePrint(arg_i));return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_PrintTwice(std::shared_ptr<TestDll::SharedAlive>* self,std::shared_ptr<TestDll::SharedAll>* arg_printer){(*self)->PrintTwice(arg_printer?*arg_printer:nullptr);}
__declspec(dllexport) double __stdcall Wrappy_SharedAlive_HalfNine(std::shared_ptr<TestDll::SharedAlive>* self,TestDll::PointerDelete* arg_pdelete){auto inner_result=(*self)->HalfNine(arg_pdelete);return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_FillWithDispose(std::shared_ptr<TestDll::SharedAlive>* self,TestDll::PointerDispose** p_arg_disposes,int l_arg_disposes){std::span<TestDll::PointerDispose*> span_arg_disposes(p_arg_disposes,l_arg_disposes);(*self)->FillWithDispose(span_arg_disposes);}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_FillWithPrint(std::shared_ptr<TestDll::SharedAlive>* self,std::shared_ptr<TestDll::SharedAll>** p_arg_printers,int l_arg_printers){std::vector<std::shared_ptr<TestDll::SharedAll>> vec_arg_printers(l_arg_printers);std::span<std::shared_ptr<TestDll::SharedAll>> span_arg_printers(&vec_arg_printers[0],l_arg_printers);for(int i=0;i<l_arg_printers;++i)if(p_arg_printers[i])vec_arg_printers[i]=*(p_arg_printers[i]);(*self)->FillWithPrint(span_arg_printers);for(int i=0;i<l_arg_printers;++i)if(vec_arg_printers[i]){if(!p_arg_printers[i]||*(p_arg_printers[i])!=vec_arg_printers[i])p_arg_printers[i]=new std::shared_ptr<TestDll::SharedAll>(vec_arg_printers[i]);}else p_arg_printers[i]=nullptr;}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_SetEvent_TwoCallback(std::shared_ptr<TestDll::SharedAlive>* self, int(__stdcall* event)(int arg_two)){(*self)->TwoCallback = event;}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_SetEvent_MakeDisposeCallback(std::shared_ptr<TestDll::SharedAlive>* self, TestDll::PointerDispose*(__stdcall* event)(TestDll::PointerDispose* arg_dispose)){(*self)->MakeDisposeCallback = event;}
__declspec(dllexport) void __stdcall Wrappy_Delete_SharedAlive(std::shared_ptr<TestDll::SharedAlive>* self){delete self;}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAliveBase>* __stdcall Wrappy_ToBase_SharedAlive(std::shared_ptr<TestDll::SharedAlive>* self){return self?new std::shared_ptr<TestDll::SharedAliveBase>(static_pointer_cast<TestDll::SharedAliveBase>(*self)):nullptr;}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAliveBase>* __stdcall Wrappy_New_SharedAliveBase(){auto inner_result=new std::shared_ptr<TestDll::SharedAliveBase>(new TestDll::SharedAliveBase());return inner_result;}
__declspec(dllexport) int __stdcall Wrappy_SharedAliveBase_One(std::shared_ptr<TestDll::SharedAliveBase>* self){auto inner_result=(*self)->One();return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_Delete_SharedAliveBase(std::shared_ptr<TestDll::SharedAliveBase>* self){delete self;}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAll>* __stdcall Wrappy_New_SharedAll(int arg_i){auto inner_result=new std::shared_ptr<TestDll::SharedAll>(new TestDll::SharedAll(arg_i));return inner_result;}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_Print(std::shared_ptr<TestDll::SharedAll>* self){(*self)->Print();}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_Write(std::shared_ptr<TestDll::SharedAll>* self,char* p_arg_l,int l_arg_l){std::span<char> span_arg_l(p_arg_l,l_arg_l);(*self)->Write(span_arg_l);}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_WriteString(std::shared_ptr<TestDll::SharedAll>* self,char* arg_s){(*self)->WriteString(std::string(arg_s));}
__declspec(dllexport) void __stdcall Wrappy_Delete_SharedAll(std::shared_ptr<TestDll::SharedAll>* self){delete self;}
}