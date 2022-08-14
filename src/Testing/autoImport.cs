namespace TestDll{internal class PointerDelete{public IntPtr? Native;public PointerDelete(IntPtr? native){Native=native;}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_New_PointerDelete();public PointerDelete(){Native=Wrappy_New_PointerDelete();}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_PointerDelete_Nothing(IntPtr self);public void Nothing(){Wrappy_PointerDelete_Nothing(Native??IntPtr.Zero);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern int Wrappy_PointerDelete_Nine(IntPtr self);public int Nine(){return Wrappy_PointerDelete_Nine(Native??IntPtr.Zero);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern float Wrappy_PointerDelete_Half(IntPtr self,float arg_a);public float Half(float a){return Wrappy_PointerDelete_Half(Native??IntPtr.Zero,a);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern double Wrappy_PointerDelete_Add(IntPtr self,int arg_l,double arg_r);public double Add(int l,double r){return Wrappy_PointerDelete_Add(Native??IntPtr.Zero,l,r);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern int Wrappy_PointerDelete_Sum(IntPtr self,int[] arg_n,int arg_s);public int Sum(int[] n,int s){return Wrappy_PointerDelete_Sum(Native??IntPtr.Zero,n,s);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_Delete_PointerDelete(IntPtr self);
~PointerDelete(){Wrappy_Delete_PointerDelete(Native??IntPtr.Zero);}
}}namespace TestDll{internal class PointerDispose:IDisposable{public IntPtr? Native;public PointerDispose(IntPtr? native){Native=native;}
private readonly object Locker=new object();
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_New_PointerDispose();public PointerDispose(){Native=Wrappy_New_PointerDispose();}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern int Wrappy_PointerDispose_Five(IntPtr self);public int Five(){if(!Native.HasValue)throw new ObjectDisposedException("PointerDispose");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("PointerDispose");return Wrappy_PointerDispose_Five(Native??IntPtr.Zero);}}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_Delete_PointerDispose(IntPtr self);
public void Dispose(){if(!Native.HasValue)throw new ObjectDisposedException("PointerDispose");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("PointerDispose");Wrappy_Delete_PointerDispose(Native.Value);Native=null;}}
}}namespace TestDll{internal class SharedAlive{public IntPtr? Native;public SharedAlive(IntPtr? native){Native=native;}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_New_SharedAlive(double arg_a);public SharedAlive(double a){Native=Wrappy_New_SharedAlive(a);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern int Wrappy_SharedAlive_Two(IntPtr self);public int Two(){return Wrappy_SharedAlive_Two(Native??IntPtr.Zero);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_SharedAlive_MakeDispose(IntPtr self);public TestDll.PointerDispose MakeDispose(){return new TestDll.PointerDispose((IntPtr?)Wrappy_SharedAlive_MakeDispose(Native??IntPtr.Zero));}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_SharedAlive_MakePrint(IntPtr self,int arg_i);public TestDll.SharedAll MakePrint(int i){return new TestDll.SharedAll((IntPtr?)Wrappy_SharedAlive_MakePrint(Native??IntPtr.Zero,i));}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_SharedAlive_PrintTwice(IntPtr self,IntPtr arg_printer);public void PrintTwice(TestDll.SharedAll printer){Wrappy_SharedAlive_PrintTwice(Native??IntPtr.Zero,printer.Native??throw new ObjectDisposedException("SharedAll"));}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern double Wrappy_SharedAlive_HalfNine(IntPtr self,IntPtr arg_pdelete);public double HalfNine(TestDll.PointerDelete pdelete){return Wrappy_SharedAlive_HalfNine(Native??IntPtr.Zero,pdelete.Native??throw new ObjectDisposedException("PointerDelete"));}
}}namespace TestDll{internal class SharedAll:IDisposable{public IntPtr? Native;public SharedAll(IntPtr? native){Native=native;}
private readonly object Locker=new object();
public bool Owner=true;
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern IntPtr Wrappy_New_SharedAll(int arg_i);public SharedAll(int i){Native=Wrappy_New_SharedAll(i);}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_SharedAll_Print(IntPtr self);public void Print(){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");Wrappy_SharedAll_Print(Native??IntPtr.Zero);}}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_SharedAll_Write(IntPtr self,byte[] arg_l);public void Write(byte[] l){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");Wrappy_SharedAll_Write(Native??IntPtr.Zero,l);}}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_SharedAll_WriteString(IntPtr self,string arg_s);public void WriteString(string s){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");Wrappy_SharedAll_WriteString(Native??IntPtr.Zero,s);}}
[System.Runtime.InteropServices.DllImport("TestDll")]private static extern void Wrappy_Delete_SharedAll(IntPtr self);
public void Dispose(){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");lock(Locker){if(!Native.HasValue)throw new ObjectDisposedException("SharedAll");if(Owner){Wrappy_Delete_SharedAll(Native.Value);Native=null;}}}
~SharedAll(){if(Native.HasValue&&Owner)Wrappy_Delete_SharedAll(Native.Value);}
}}