#include"PointerDelete.h"
#include"PointerDispose.h"
#include"SharedAlive.h"
#include"SharedAll.h"
#include<memory>
#include<string>
extern "C"{
__declspec(dllexport) TestDll::PointerDelete* __stdcall Wrappy_New_PointerDelete(){return new TestDll::PointerDelete();}
__declspec(dllexport) void __stdcall Wrappy_PointerDelete_Nothing(TestDll::PointerDelete* self){self->Nothing();}
__declspec(dllexport) int __stdcall Wrappy_PointerDelete_Nine(TestDll::PointerDelete* self){return self->Nine();}
__declspec(dllexport) float __stdcall Wrappy_PointerDelete_Half(TestDll::PointerDelete* self,float arg_a){return self->Half(arg_a);}
__declspec(dllexport) double __stdcall Wrappy_PointerDelete_Add(TestDll::PointerDelete* self,int arg_l,double arg_r){return self->Add(arg_l,arg_r);}
__declspec(dllexport) int __stdcall Wrappy_PointerDelete_Sum(TestDll::PointerDelete* self,int* arg_n,int arg_s){return self->Sum(arg_n,arg_s);}
__declspec(dllexport) void __stdcall Wrappy_Delete_PointerDelete(TestDll::PointerDelete* self){{delete self;}}
__declspec(dllexport) TestDll::PointerDispose* __stdcall Wrappy_New_PointerDispose(){return new TestDll::PointerDispose();}
__declspec(dllexport) int __stdcall Wrappy_PointerDispose_Five(TestDll::PointerDispose* self){return self->Five();}
__declspec(dllexport) void __stdcall Wrappy_Delete_PointerDispose(TestDll::PointerDispose* self){{delete self;}}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAlive>* __stdcall Wrappy_New_SharedAlive(double arg_a){return new std::shared_ptr<TestDll::SharedAlive>(new TestDll::SharedAlive(arg_a));}
__declspec(dllexport) int __stdcall Wrappy_SharedAlive_Two(std::shared_ptr<TestDll::SharedAlive>* self){return (*self)->Two();}
__declspec(dllexport) TestDll::PointerDispose* __stdcall Wrappy_SharedAlive_MakeDispose(std::shared_ptr<TestDll::SharedAlive>* self){return (*self)->MakeDispose();}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAll>* __stdcall Wrappy_SharedAlive_MakePrint(std::shared_ptr<TestDll::SharedAlive>* self,int arg_i){return new std::shared_ptr<TestDll::SharedAll>((*self)->MakePrint(arg_i));}
__declspec(dllexport) void __stdcall Wrappy_SharedAlive_PrintTwice(std::shared_ptr<TestDll::SharedAlive>* self,std::shared_ptr<TestDll::SharedAll>* arg_printer){(*self)->PrintTwice(*arg_printer);}
__declspec(dllexport) double __stdcall Wrappy_SharedAlive_HalfNine(std::shared_ptr<TestDll::SharedAlive>* self,TestDll::PointerDelete* arg_pdelete){return (*self)->HalfNine(arg_pdelete);}
__declspec(dllexport) std::shared_ptr<TestDll::SharedAll>* __stdcall Wrappy_New_SharedAll(int arg_i){return new std::shared_ptr<TestDll::SharedAll>(new TestDll::SharedAll(arg_i));}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_Print(std::shared_ptr<TestDll::SharedAll>* self){(*self)->Print();}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_Write(std::shared_ptr<TestDll::SharedAll>* self,char* arg_l){(*self)->Write(arg_l);}
__declspec(dllexport) void __stdcall Wrappy_SharedAll_WriteString(std::shared_ptr<TestDll::SharedAll>* self,char* arg_s){(*self)->WriteString(std::string(arg_s));}
__declspec(dllexport) void __stdcall Wrappy_Delete_SharedAll(std::shared_ptr<TestDll::SharedAll>* self){{delete self;}}
}