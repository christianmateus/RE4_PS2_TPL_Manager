using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager
{
    public class TexturePreviewBox : PictureBox
    {
        private float manualZoom = 0f;
        private PointF pan = PointF.Empty;
        private Point dragStart;
        private PointF panStart;
        private bool dragging;

        public TexturePreviewBox()
        {
            DoubleBuffered = true; ResizeRedraw = true; TabStop = true;
            BackColor = Color.FromArgb(24,24,24); SizeMode = PictureBoxSizeMode.Normal;
            MouseWheel += PreviewMouseWheel;
        }

        public new Image Image
        {
            get { return base.Image; }
            set
            {
                if (ReferenceEquals(base.Image, value))
                    return;

                base.Image = value;
                ResetView();
            }
        }

        public void ResetView() { manualZoom = 0f; pan = PointF.Empty; Invalidate(); }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); Focus(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if(e.Button==MouseButtons.Left){dragging=true;dragStart=e.Location;panStart=pan;Cursor=Cursors.Hand;} }
        protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); if(dragging){pan=new PointF(panStart.X+e.X-dragStart.X,panStart.Y+e.Y-dragStart.Y);Invalidate();} }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); dragging=false; Cursor=Cursors.Default; }
        protected override void OnMouseDoubleClick(MouseEventArgs e) { base.OnMouseDoubleClick(e); ResetView(); }

        private void PreviewMouseWheel(object sender, MouseEventArgs e)
        {
            if(Image==null) return;
            float current = manualZoom > 0 ? manualZoom : GetAutoScale();
            float next;
            if(e.Delta > 0) next = current >= 1f ? (float)Math.Min(32, Math.Floor(current)+1) : Math.Min(1f, current*1.25f);
            else next = current > 1f ? Math.Max(1f, (float)Math.Ceiling(current)-1) : Math.Max(0.1f, current/1.25f);
            manualZoom=next; Invalidate();
        }

        private float GetAutoScale()
        {
            if(Image==null) return 1f;
            Rectangle v=Rectangle.Inflate(ClientRectangle,-8,-8);
            if(Image.Width<=v.Width && Image.Height<=v.Height)
                return Math.Max(1,Math.Min(v.Width/Image.Width,v.Height/Image.Height));
            return (float)Math.Min((double)v.Width/Image.Width,(double)v.Height/Image.Height);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            DrawCheckerboard(pe.Graphics,ClientRectangle);
            if(Image==null){DrawEmptyText(pe.Graphics);return;}
            Rectangle v=Rectangle.Inflate(ClientRectangle,-8,-8); if(v.Width<=0||v.Height<=0)return;
            float scale=manualZoom>0?manualZoom:GetAutoScale();
            int w=Math.Max(1,(int)Math.Round(Image.Width*scale)), h=Math.Max(1,(int)Math.Round(Image.Height*scale));
            Rectangle dest=new Rectangle(v.Left+(v.Width-w)/2+(int)pan.X,v.Top+(v.Height-h)/2+(int)pan.Y,w,h);
            pe.Graphics.InterpolationMode=scale>=1f?InterpolationMode.NearestNeighbor:InterpolationMode.HighQualityBicubic;
            pe.Graphics.PixelOffsetMode=scale>=1f?PixelOffsetMode.Half:PixelOffsetMode.HighQuality;
            pe.Graphics.DrawImage(Image,dest,0,0,Image.Width,Image.Height,GraphicsUnit.Pixel);
            DrawInfoBadge(pe.Graphics,scale);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent) { }
        private static void DrawCheckerboard(Graphics g,Rectangle b){g.Clear(Color.FromArgb(24,24,24));const int t=12;using(var a=new SolidBrush(Color.FromArgb(54,54,54)))using(var d=new SolidBrush(Color.FromArgb(40,40,40)))for(int y=0;y<b.Height;y+=t)for(int x=0;x<b.Width;x+=t)g.FillRectangle(((x/t+y/t)%2==0)?a:d,x,y,t,t);}
        private void DrawInfoBadge(Graphics g,float z){string text=Image.Width+"×"+Image.Height+"  •  "+z.ToString(z>=1?"0.#":"0.00")+"x  •  wheel: zoom  drag: pan  double-click: fit";using(var f=new Font("Segoe UI",8.25f)) {var s=g.MeasureString(text,f);var r=new RectangleF(8,ClientSize.Height-s.Height-14,s.Width+14,s.Height+6);using(var bg=new SolidBrush(Color.FromArgb(200,18,18,18)))using(var fg=new SolidBrush(Color.Gainsboro)){g.FillRectangle(bg,r);g.DrawString(text,f,fg,r.Left+7,r.Top+3);}}}
        private void DrawEmptyText(Graphics g){const string text="Select a texture to preview";using(var f=new Font("Segoe UI",9f))using(var b=new SolidBrush(Color.FromArgb(160,200,200,200))){var s=g.MeasureString(text,f);g.DrawString(text,f,b,(ClientSize.Width-s.Width)/2f,(ClientSize.Height-s.Height)/2f);}}
    }
}
