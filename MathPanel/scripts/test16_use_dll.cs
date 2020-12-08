//test16_use_dll
var outputAssembly = @"C:\C_devel\utils\MathPanel\dll\quadroequ.dll";
Dynamo.Console(string.Format("Try Assembly:{0}", outputAssembly));
System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(outputAssembly);
Type type = assembly.GetType("MathPanelExt.QuadroEqu");
System.Reflection.MethodInfo method = type.GetMethod("Solve");
double x1 = 0, x2 = 0;
object[] pars = new object[5];
pars[0] = 1.0;
pars[1] = 0.0;
pars[2] = -1.0;
pars[3] = null;
pars[4] = null;
object res = method.Invoke(null, pars);
//bool blResult = (bool)res;
//if (blResult)
{
    x1 = (double) pars[3];
    x2 = (double) pars[4];
}
Dynamo.Console(string.Format("x1 {0}, x2 {1}", x1, x2));