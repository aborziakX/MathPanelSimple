//2020, Andrei Borziak
//A simple module for drawing graphs on html-canvas, the top left pixel has position of (0, 0)
if (typeof GRAPHIX == "undefined") {
    GRAPHIX = {
//members
        PADDING: 20,        //padding
        AX_BG: "#000000",   //background
        AX_CL: "#00FF00",   //color of the axes
        borders: 3, //logical OR: 1-horizontal, 2-vertical, 4-horizontal up, 8-vertical right
        displaySubzero: false, //if the size is less than 1, if displaySubzero == true then the size becomes 1, otherwise 0
//methods
        //to fill the background, draw the axis
        drawAxes: function(canvas) {
            var cnv = document.getElementById(canvas);//get canvas by id
            var w = cnv.width;//get width
            var h = cnv.height;//get height
            var ctx = cnv.getContext('2d');//get context
 //fill the canvas
            ctx.fillStyle = this.AX_BG;
            ctx.fillRect(0, 0, w, h);
            ctx.strokeStyle = this.AX_CL;
            //if (typeof bAll === "undefined") return;
            if( this.borders == 0 ) return;
            ctx.lineWidth = 1;//line width
            ctx.beginPath();
            //vertical
            if( (this.borders & 0x2) > 0 ) {
                ctx.moveTo(this.PADDING, this.PADDING);
                ctx.lineTo(this.PADDING, h - this.PADDING);
                ctx.stroke();
            }
            //horizontal
            if( (this.borders & 0x1) > 0 ) {
                ctx.moveTo(this.PADDING, h - this.PADDING);
                ctx.lineTo(w - this.PADDING, h - this.PADDING);
                ctx.stroke();
            }
            //vertical right
            if( (this.borders & 0x8) > 0 ) {
                ctx.moveTo(w - this.PADDING, this.PADDING);
                ctx.lineTo(w - this.PADDING, h - this.PADDING);
                ctx.stroke();
            }
            //horizontal up
            if( (this.borders & 0x4) > 0 ) {
                ctx.moveTo(this.PADDING, this.PADDING);
                ctx.lineTo(w - this.PADDING, this.PADDING);
                ctx.stroke();
            }
        },

        //to display the data
        //canvas - id, data - 2 dimensional array of data, opt - optional parameters
        drawData: function(canvas, data, opt) {
            var cnv = document.getElementById(canvas);
            var w = cnv.width - 2 * this.PADDING;   //the width without padding
            var h = cnv.height - 2 * this.PADDING;  //the height without padding
            var x0 = opt.x0;    //minimum value by X-axis
            var x1 = opt.x1;    //maximum value by X-axis
            var y0 = opt.y0;    //minimum value by Y-axis
            var y1 = opt.y1;    //maximum value by Y-axis
            var dx = (x1 - x0);
            var dy = (y1 - y0);
            var sz = "" + opt.size; //size
            if (sz == "undefined") sz = 1;
            else sz = sz * 1;
            var clr = "" + opt.clr; //the main color
            if( clr == "undefined" ) clr = "#ff0000";
            var colorstroke = "" + opt.csk; //the stroke color
            if( colorstroke == "undefined" ) colorstroke = clr;
            var linewidth = "" + opt.lnw;   //the line width
            if (linewidth == "undefined") linewidth = 1;
            else linewidth = linewidth * 1;
            var sz_old;
            var style = opt.sty;    //style
            var bLine = false;
            var height = 10;
            var text = "";
            var fromzero = "" + opt.fromzero;
            if (fromzero == "undefined") fromzero = 0;
            var textonly = "" + opt.textonly;
            if (textonly == "undefined") textonly = 0;           
            var textdown = "" + opt.textdown;
            if (textdown == "undefined") textdown = 0; 
            var fontsize = "" + opt.fontsize;//the font size
            if (fontsize == "undefined") fontsize = 10; 

            //get Context
            var ctx = cnv.getContext('2d');
            ctx.beginPath();
            ctx.lineWidth = linewidth;
            ctx.strokeStyle = colorstroke;
            ctx.fillStyle = clr;
            ctx.font = fontsize + "px Verdana";
            //alert(clr + "," + colorstroke);

            var sqrt2_2 = Math.sqrt(2) * 0.3;
            var m0;

            var bRestoreSize = false, bRestoreColor = false, bRestoreStyle = false, bRestoreLw = false, bRestoreColorstroke = false, bRestoreFont = false;
            //loop through data
            for (var i = 0; i < data.length; i++) {
                var r = data[i];//the current row in data
                var x = r[0];   //the position by X-axis
                var y = r[1];   //the position by Y-axis
                if( r.length > 2 && r[2] != null ) {//size
                    bRestoreSize = true;
                    sz_old = sz;
                    sz = r[2];
                }
                if( r.length > 3 && r[3] != null ) {//fill style
                    bRestoreColor = true;
                    ctx.fillStyle = r[3];
                }
                if( r.length > 4 && r[4] != null ) {//draw style
                    bRestoreStyle = true;
                    style = r[4];
                }
                if( r.length > 5 && r[5] != null ) {//height
                    height = r[5];
                }
                if( r.length > 6 && r[6] != null ) {//text
                    text = r[6];
                }
                else text = "";

                if( r.length > 7 && r[7] != null ) {//line width
                    ctx.lineWidth = r[7];
                    bRestoreLw = true;
                }
                if (r.length > 8 && r[8] != null) {//stroke style
                    ctx.strokeStyle = r[8];
                    bRestoreColorstroke = true;
                }
                if (r.length > 9 && r[9] != null) {//font
                    ctx.font = r[9] + "px Verdana";
                    bRestoreFont = true;
                }

                //calculate the positions
                var l = Math.round((w * (x - x0)) / dx + this.PADDING); //the position on canvas by X-axis
                var m = Math.round((h - (h * (y - y0)) / dy) + this.PADDING);//the position on canvas by X-axis
                //apply the style
                if (style == "dots") {  //dots
                    ctx.fillRect(l-sz/2, m-sz/2, sz, sz);
                    if (text != "") ctx.fillText(text, l, m - 10);
                }
                else if (style == "circle") {//circles
                    ctx.beginPath();
                    ctx.arc(l, m, sz, 0, 2 * Math.PI);
                    ctx.stroke();
                    ctx.fill();
                    if (text != "") ctx.fillText(text, l, m - 10);
                }
                else if (style == "hist") {//histogram
                    if (fromzero == 0) {
                        ctx.moveTo(l, h + this.PADDING);
                    } else {
                        m0 = Math.round((h - (h * (0 - y0)) / dy) + this.PADDING);
                        ctx.moveTo(l, m0);
                    }
                    ctx.lineTo(l, m);
                    ctx.stroke();
                    if (text != "") ctx.fillText(text, l, m - 10);
                }
                else if (style == "hist_3") {//three dimensional histogram
                    if (textonly != 1) {
                        ctx.beginPath();
                        if (fromzero == 0) {
                            m0 = h + this.PADDING;
                            ctx.moveTo(l, m0);
                        } else {
                            m0 = Math.round((h - (h * (0 - y0)) / dy) + this.PADDING);
                            ctx.moveTo(l, m0);
                        }
                        ctx.lineTo(l, m);
                        ctx.lineTo(l + sz, m);
                        ctx.lineTo(l + sz, m0);
                        ctx.closePath();
                        ctx.fill();
                        ctx.stroke();

                        ctx.beginPath();
                        ctx.moveTo(l, m);
                        ctx.lineTo(l + sz, m);
                        ctx.lineTo(l + sz + sz * sqrt2_2, m - sz * sqrt2_2);
                        ctx.lineTo(l + sz * sqrt2_2, m - sz * sqrt2_2);
                        ctx.closePath();
                        ctx.fill();
                        ctx.stroke();

                        ctx.beginPath();
                        ctx.moveTo(l + sz, m);
                        ctx.lineTo(l + sz + sz * sqrt2_2, m - sz * sqrt2_2);
                        ctx.lineTo(l + sz + sz * sqrt2_2, m0 - sz * sqrt2_2);
                        ctx.lineTo(l + sz, m0);
                        ctx.closePath();
                        ctx.fill();
                        ctx.stroke();
                    }
                    if (text != "") ctx.fillText(text, l, textdown != 1 ? m - 10 : m0 + (10 + (i % 4) * 12));
                    //if (i == 0) alert("l=" + l + ", m0=" + m0 + ", m=" + m + ", sz=" + sz + ", l2=" + (l + sz));
                }
                else if (style == "hist3d") {//histogram
                    ctx.moveTo(l, m);
                    ctx.lineTo(l, m - height);
                    ctx.stroke();
                    if( text != "" ) ctx.fillText(text, l, m - height - 10);
                }
                else if (style == "line" || style == "line_end" || style == "line_endf") {//lines
                    if (!bLine) {
                        ctx.beginPath();
                        ctx.moveTo(l, m);
                        bLine = true;
                    }
                    else {
                        ctx.lineTo(l, m);
                    }
                    if(style == "line_end" || style == "line_endf") {
                        bLine = false;
                        if( style == "line_endf") ctx.fill();
                        //else 
                        ctx.stroke();
                    }                  
                    if (text != "") ctx.fillText(text, l, m - 10);
                }
                //restore settings
                if(bRestoreSize) {
                    bRestoreSize = false;
                    sz = sz_old;
                }
                if(bRestoreColor) {
                    bRestoreColor = false;
                    ctx.fillStyle = clr;
                }
                if(bRestoreStyle) {
                    bRestoreStyle = false;
                    style = opt.sty;
                }                
                if(bRestoreLw) {
                    bRestoreLw = false;
                    ctx.lineWidth = linewidth;
                }
                if (bRestoreColorstroke) {
                    bRestoreColorstroke = false;
                    ctx.strokeStyle = colorstroke;
                }
                if (bRestoreFont) {
                    bRestoreFont = false;
                    ctx.font = fontsize + "px Verdana";
                }
            }
            if (bLine) ctx.stroke();
            //alert(bLine);
        },

        //display the text
        drawText: function(canvas, title, x, y, color, size) {
            var cnv = document.getElementById(canvas);
            var ctx = cnv.getContext('2d');
            ctx.fillStyle = color;
            ctx.font = size + "px Verdana";
            ctx.fillText(title, x, y);
        },

        //display the image
        drawImage: function(canvas, imgid, x, y) {
            var cnv = document.getElementById(canvas);
            var ctx = cnv.getContext('2d');
            var img = document.getElementById(imgid);
            if( img != null ) ctx.drawImage(img, x, y);
        },
        
        //display the json-string
        drawJson: function (canvas, w) {
            var s = JSON.parse(w);//eval(w);
            var dpa = new Date().getTime();
            this.drawJsonObj(canvas, s);
            return dpa;
        },

        //display the json-object
        drawJsonObj: function (canvas, s) {
            //get the canvas
            var cnv = document.getElementById(canvas);
            //get the context
            var ctx = cnv.getContext('2d');
            //alert(s + ",o3=" + s["options"]["x0"] + ",o2=" + s["data"][0]["x"]);
            //get optional parameters
            var opt = s["options"];

            //get the width and height for canvas
            width_x = (opt["wid"]);
            height_x = (opt["hei"]);
            if (cnv.width != width_x) {
                cnv.width = width_x;
            }
            if (cnv.height != height_x) {
                cnv.height = height_x;
            }
            //get the scale factor
            var scale = cnv.width / (opt["x1"] - opt["x0"]);
            //start building the two dimensional array
            var dd = new Array();
            for (var i = 0; i < s["data"].length; i++) {
            //get a row of data
                var row = s["data"][i];
                //get positions
                var x = row["x"];
                var y = row["y"];
                //get size
                var sz = ("" + row["rad"]);
                if( sz == "undefined" ) sz = null;
                else {
                    sz = sz * scale;
                    if (sz < 1) sz = this.displaySubzero ? 1 : 0;
                }
                //get the color
                var clr = ("" + row["clr"]);
                if( clr == "undefined" ) clr = null;
                //get the style
                var style = ("" + row["sty"]);
                if (style == "undefined") style = null;
                //get the height
                var height = ("" + row["hei"]);
                if (height == "undefined") height = null;
                //get the text
                var text = ("" + row["txt"]);
                if (text == "undefined") text = null;
                //get the line width
                var linewidth = ("" + row["lnw"]);
                if (linewidth == "undefined") linewidth = null;
                //get the stroke color
                var colorstroke = ("" + row["csk"]);
                if (colorstroke == "undefined") colorstroke = null;
                //get the fontsize
                var fontsize = ("" + row["fontsize"]);
                if (fontsize == "undefined") fontsize = null;
                //push a new one dimensional array into currrent two dimensional array
                dd.push(new Array(x, y, sz, clr, style, height, text, linewidth, colorstroke, fontsize));
            }

            if( ("" + opt["bg"]) != "undefined" ) { //set the background color
                this.AX_BG = opt["bg"];
            }

            if( ("" + opt["second"]) == "undefined" ) { //draw axes    
                this.AX_CL = "#ffff00";
                this.PADDING = 5;
                this.borders = 0;//15;
                this.drawAxes(canvas);
            }
            if (("" + opt["img"]) != "undefined") {//set the background image
                var obj = document.getElementById("img1");
                if (obj != null) try {
                    obj.src = opt["img"];
                    this.drawImage(canvas, "img1", 0, 0);
                } catch (e) {  };
            }
            //draw our data
            this.drawData(canvas, dd, opt);
        },

        dumb:0
    }
}