//test18_bitmap
System.Drawing.Color[] clrs =
{
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
    System.Drawing.Color.AliceBlue,
    System.Drawing.Color.Aqua,
    System.Drawing.Color.Gainsboro,
};
//Dynamo.SceneClear();
var s1 = MathPanelExt.QuadroEqu.DrawBitmap(15, 20, clrs );
string s2 = "{\"options\":{\"x0\": -0.5, \"x1\": 20, \"y0\": -0.5, \"y1\": 15, \"clr\": \"#ff0000\", \"sty\": \"dots\", \"size\":40, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"img\":\"http://www.pvobr.ru/images/russia.png\" }";
s2 += ", \"data\":[" + s1 + "]}";
Dynamo.SceneJson(s2);
//имеется задержка в рисовании img, поэтому заснуть и повторить
System.Threading.Thread.Sleep(300);
Dynamo.SceneJson(s2);

var s3 = MathPanelExt.QuadroEqu.DrawBitmap(15, 20, clrs, 0, 5 );
string s4 = "{\"options\":{\"x0\": -0.5, \"x1\": 20, \"y0\": -0.5, \"y1\": 15, \"clr\": \"#ff0000\", \"sty\": \"dots\", \"size\":40, \"lnw\": 3, \"wid\": 800, \"hei\": 600, \"second\":1 }";
s4 += ", \"data\":[" + s3 + "]}";
Dynamo.SceneJson(s4, true);
    
