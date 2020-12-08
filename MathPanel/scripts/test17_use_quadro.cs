//test17_use_quadro
double x1 = 0, x2 = 0;
MathPanelExt.QuadroEqu.Solve(1, 0, -1, out x1, out x2 );
Dynamo.Console(string.Format("x1 {0}, x2 {1}", x1, x2));

var s1 = MathPanelExt.QuadroEqu.DrawRange(1, 0, -1, -2, 2, 24 );
string s2 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#ff0000\", \"sty\": \"circle\", \"size\":10, \"lnw\": 3, \"wid\": 800, \"hei\": 600 }";
s2 += ", \"data\":[" + s1 + "]}";
Dynamo.SceneJson(s2);
    
var s3 = MathPanelExt.QuadroEqu.DrawRange(-2, 0, 1, -1.8, 2, 24);
string s4 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#00ff00\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s4 += ", \"data\":[" + s3 + "]}";
Dynamo.SceneJson(s4, true);

var s5 = MathPanelExt.QuadroEqu.DrawEllipse(1.1, 1, -1.7, 4, 0, Math.PI * 2, 24);
string s6 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#0000ff\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s6 += ", \"data\":[" + s5 + "]}";
Dynamo.SceneJson(s6, true);

var s7 = MathPanelExt.QuadroEqu.DrawEllipse(1, 2.5, 1.5, 4, 0, Math.PI * 2, 24);
string s8 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#ffaa00\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s8 += ", \"data\":[" + s7 + "]}";
Dynamo.SceneJson(s8, true);

var s9 = MathPanelExt.QuadroEqu.DrawLine(-2.5, 6, 1.5, 7);
string s10 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#ffff00\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s10 += ", \"data\":[" + s9 + "]}";
Dynamo.SceneJson(s10, true);

var s11 = MathPanelExt.QuadroEqu.DrawBezier2(0, 4, 1.5, 7, 3, 5, 10);
string s12 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#ff00ff\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s12 += ", \"data\":[" + s11 + "]}";
Dynamo.SceneJson(s12, true);

var s13 = MathPanelExt.QuadroEqu.DrawBezier3(-3, 6, -1, 10, 1, 5, 3, 7, 20);
string s14 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#ffffff\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s14 += ", \"data\":[" + s13 + "]}";
Dynamo.SceneJson(s14, true);

var s15 = MathPanelExt.QuadroEqu.DrawLine(-0.5, 2, 0.3, 6);
string s16 = "{\"options\":{\"x0\": -3, \"x1\": 3, \"y0\": -3, \"y1\": 9, \"clr\": \"#00ffff\", \"sty\": \"line\", \"size\":5, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s16 += ", \"data\":[" + s15 + "]}";
Dynamo.SceneJson(s16, true);


    
