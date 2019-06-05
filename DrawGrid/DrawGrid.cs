using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace DrawGrid
{
    public partial class DrawGrid : UserControl
    {
        public List<Row> table = null;

        public List<Column> columns = new List<Column>();

        int offsetX = 0;

        int offsetY = 0;

        int rowHeight = 22;
        int headerHeight = 25;

        int rowHead = 25;

        int totalWidth = 25;

        int gridWidth = 0;
        int gridHeight = 0;

        int rowsPerView = 0;

        int mouseCell = 0;
        int mouseRow = 0;

        int anchor;


        Font headerFont;
        StringFormat headerFormat;
        StringFormat cellFormat;

        Pen headerPen;

        Pen gray;

        SolidBrush rowAlternateColor;
        SolidBrush headerOverColor;
        SolidBrush rowSelectedColor;
        SolidBrush rowHeadSelectedColor;

        Pen scrollColor;
        SolidBrush scrollFillColor;
        SolidBrush scrollFillColorMouse;


        bool hScrollVisible = true;
        int hScrollCursorPos = 0;
        int hScrollCursorSize = 100;
        bool hScrollMouse = false;
        decimal hScrollValue = 0;
        decimal hScrollOffX = 0;

        int scrollMouseOffset = 0;

        bool vScrollVisible = true;
        int vScrollCursorPos = 0;
        int vScrollCursorSize = 100;
        bool vScrollMouse = false;
        decimal vScrollValue = 0;
        decimal vScrollOffY = 0;


        public DrawGrid()
        {
            headerFont = new Font("Tahoma", 9);
            headerFormat = new StringFormat(StringFormatFlags.NoWrap);
            headerFormat.LineAlignment = StringAlignment.Center;
            headerPen = new Pen(Color.FromArgb(223, 223, 223));
            headerOverColor = new SolidBrush(Color.FromArgb(217, 235, 249));
            rowSelectedColor = new SolidBrush(Color.FromArgb(0, 120, 215));
            rowHeadSelectedColor = new SolidBrush(Color.FromArgb(188, 220, 244));


            scrollColor = new Pen(Color.FromArgb(255, 222, 222, 222));


            scrollFillColor = new SolidBrush(Color.FromArgb(255, 193, 193, 193));
            scrollFillColorMouse = new SolidBrush(Color.FromArgb(255, 211, 211, 211));


            cellFormat = new StringFormat(StringFormatFlags.NoWrap);
            cellFormat.Trimming = StringTrimming.EllipsisCharacter;
            cellFormat.LineAlignment = StringAlignment.Center;

            gray = new Pen(Color.FromArgb(160, 160, 160));
            rowAlternateColor = new SolidBrush(Color.FromArgb(245, 245, 245));

            anchor = -1;


            InitializeComponent();
            this.grid.MouseWheel += grid_MouseWheel;
        }

        private void grid_MouseWheel(object sender, MouseEventArgs e)
        {

            vScrollValue -= Math.Sign(e.Delta) * 3 / (decimal)(table.Count - rowsPerView);

            if (vScrollValue < 0) vScrollValue = 0;
            if (vScrollValue > 1) vScrollValue = 1;

            vScrollCursorPos = (int)((gridHeight - vScrollCursorSize - 3) * vScrollValue);


            this.Refresh();
        }

        public void addColumn(string text, int width, int index)
        {
            columns.Add(new Column() { text = text, width = width, index = index });

            totalWidth += width;

        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void DrawGrid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(171,171,171));

            DrawHeaders(e.Graphics);

            DrawRows(e.Graphics);



            

            GraphicsPath path;
            //Horizontal Scroll
            if (hScrollVisible)
            {
                e.Graphics.DrawRectangle(Pens.LightGray, 0, gridHeight, gridWidth, 15);
                e.Graphics.FillRectangle(Brushes.White, 0, gridHeight, gridWidth, 15);

                path = RoundedRect(new Rectangle(1 + hScrollCursorPos, gridHeight + 2, hScrollCursorSize, 10), 5);

                e.Graphics.FillPath(hScrollMouse ? scrollFillColorMouse : scrollFillColor, path);
                e.Graphics.DrawPath(scrollColor, path);
            }

            //Vertical Scroll
            if (vScrollVisible)
            {
                e.Graphics.DrawRectangle(Pens.LightGray, gridWidth, 0, 15, gridHeight);
                e.Graphics.FillRectangle(Brushes.White, gridWidth, 0, 15, gridHeight);

                path = RoundedRect(new Rectangle(gridWidth + 2, 1 + vScrollCursorPos, 10, vScrollCursorSize), 5);
                e.Graphics.FillPath(vScrollMouse ? scrollFillColorMouse : scrollFillColor, path);
                e.Graphics.DrawPath(scrollColor, path);
            }

            //Corner
            if (hScrollVisible && vScrollVisible)
            {
                e.Graphics.FillRectangle(Brushes.White, gridWidth, gridHeight, 15, 15);
            }
        }


        void DrawHeaders(Graphics g)
        {
            hScrollOffX = (totalWidth - gridWidth) * -hScrollValue;

            int offX = 1 + rowHead + (int)hScrollOffX;

            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                Column col = columns[colIndex];
                if (offX + col.width > 0 && offX < gridWidth && colIndex > anchor)
                {

                    g.DrawRectangle(headerPen, offX, 1, col.width, headerHeight);
                    g.FillRectangle(Brushes.White, offX + 1, 2, col.width - 1, headerHeight - 1);

                    g.DrawString(col.text, headerFont, Brushes.Black, new RectangleF(offX + 8, 1, col.width, 25), headerFormat);
                }
                offX += col.width;
            }

            offX = 1 + rowHead;
            for (int colIndex = 0; colIndex <= anchor; colIndex++)
            {
                Column col = columns[colIndex];
                if (offX + col.width > 0 && offX < gridWidth)
                {

                    g.DrawRectangle(headerPen, offX, 1, col.width, headerHeight);
                    g.FillRectangle(Brushes.White, offX + 1, 2, col.width - 1, headerHeight - 1);

                    g.DrawString(col.text, headerFont, Brushes.Black, new RectangleF(offX + 8, 1, col.width, 25), headerFormat);
                }
                offX += col.width;
            }

            g.DrawRectangle(headerPen, 1, 1, rowHead, headerHeight);
            g.FillRectangle(Brushes.White, 2, 2, rowHead - 1, headerHeight - 1);
            g.DrawLine(gray, 0, headerHeight, 0, 0);


        }
        void DrawRows(Graphics g)
        {
            if (table != null)
            {

                int scrollIndex = (int)((table.Count - rowsPerView) * vScrollValue);
                if (scrollIndex < 0) scrollIndex = 0;

                for (int index = scrollIndex; index <= scrollIndex + rowsPerView && index < table.Count; index++)
                {
                    DrawRow(g, table[index], index);
                }
            }
        }
        void DrawRow(Graphics g, Row row, int index)
        {
            int offX = 1 + rowHead + (int)hScrollOffX;
            vScrollOffY = (table.Count - rowsPerView) * vScrollValue;
            int offY = headerHeight + 1 + (index - (int)vScrollOffY) * rowHeight;


            //Dibujo las filas, siempre y cuando este en al portview y que no sean anchor ya que esas se dibujan despues
            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                Column col = columns[colIndex];

                if (offX + col.width > 0 && offX < gridWidth && colIndex > anchor)
                {
                    DrawCell(g, col, row, index, offX, offY);
                }
                offX += col.width;
            }
            //Ahora dibujo las anchor
            offX = 1 + rowHead;
            for (int colIndex = 0; colIndex <= anchor; colIndex++)
            {
                Column col = columns[colIndex];

                if (offX + col.width > 0 && offX < gridWidth)
                {
                    DrawCell(g, col, row, index, offX, offY);
                }
                offX += col.width;
            }

            g.DrawRectangle(headerPen, 1, offY, rowHead, rowHeight);

            g.FillRectangle(row.selected ? rowHeadSelectedColor : Brushes.White, 2, offY + 1, rowHead - 1, rowHeight - 1);

 

            g.DrawLine(gray, 0, offY, 0, offY + rowHeight);
            g.DrawLine(gray, rowHead + 1, offY, rowHead + 1, offY + rowHeight);


        }

        void DrawCell(Graphics g, Column col, Row row, int index, int offX, int offY)
        {
            g.DrawRectangle(gray, offX, offY, col.width, rowHeight);
            g.FillRectangle(row.selected ? rowSelectedColor : (index % 2 == 0 ? Brushes.White : rowAlternateColor), offX + 1, offY + 1, col.width - 1, rowHeight - 1);
            g.DrawString(row.data[col.index].ToString(), headerFont, row.selected ? Brushes.White : Brushes.Black, new RectangleF(offX + 2, offY + 1, col.width, rowHeight), cellFormat);
        }

        private void hScroll_Scroll(object sender, ScrollEventArgs e)
        {
            grid.Refresh();
        }

        void calculateSizes()
        {
            rowsPerView = ((gridHeight - headerHeight) / rowHeight);

            if (table != null && table.Count > rowsPerView)
            {
                vScrollVisible = true;
            }
            else
            {
                vScrollVisible = false;
                vScrollValue = 0;
            }

            if (grid.Width - (vScrollVisible ? 15 : 0) < totalWidth)
            {
                hScrollVisible = true;
            }
            else
            {
                hScrollVisible = false;
                hScrollValue = 0;
            }

            gridWidth = (vScrollVisible ? grid.Width - 15 : grid.Width);
            gridHeight = (hScrollVisible ? grid.Height - 15 : grid.Height);



            if (totalWidth > gridWidth) hScrollValue = -hScrollOffX / (totalWidth - gridWidth);
            if (hScrollValue > 1) hScrollValue = 1;


            if (table != null && table.Count > rowsPerView) vScrollValue = vScrollOffY / (table.Count - rowsPerView);
            if (vScrollValue > 1) vScrollValue = 1;

            if (totalWidth > gridWidth) hScrollCursorSize = gridWidth - (totalWidth - gridWidth);
            if (hScrollCursorSize < 30) hScrollCursorSize = 30;
            if (table != null && table.Count > rowsPerView) vScrollCursorSize = gridHeight - (int)(20 * (table.Count - rowsPerView));
            if (vScrollCursorSize < 30) vScrollCursorSize = 30;


            hScrollCursorPos = (int)((gridWidth - hScrollCursorSize - 3) * hScrollValue);
            vScrollCursorPos = (int)((gridHeight - vScrollCursorSize - 3) * vScrollValue);
        }

        private void DrawGrid_Resize(object sender, EventArgs e)
        {
            calculateSizes();


            grid.Refresh();

        }
        public void SetData(DataTable dt)
        {
           
            table = new List<Row>();
            foreach (DataRow row in dt.Rows )
            {
                table.Add(new Row(row));
            }
            calculateSizes();
        }


        private void grid_Move(object sender, EventArgs e)
        {
        
        }

        private void grid_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.None)
            {
                if (e.Y <= headerHeight && e.X <= gridWidth )
                {
                    mouseRow = -1;
                    if (e.X <= rowHead)
                    {
                        mouseCell = -1;
                    } else if (e.X < totalWidth)
                    {
                        mouseCell = 1;
                    }
                }
                else
                {

                    hScrollMouse = hScrollVisible && e.Y >= gridHeight && e.X > hScrollCursorPos && e.X <= hScrollCursorPos + hScrollCursorSize;
                    vScrollMouse = vScrollVisible && e.X >= gridWidth && e.Y > vScrollCursorPos && e.Y <= vScrollCursorPos + vScrollCursorSize;

                }

                this.Refresh();
            }
            else if (e.Button == MouseButtons.Left && hScrollMouse)
            {
                hScrollCursorPos = scrollMouseOffset + e.X;
                int max = gridWidth  - hScrollCursorSize - 3;
                if (hScrollCursorPos > max) hScrollCursorPos = max;
                if (hScrollCursorPos < 0) hScrollCursorPos = 0;

                hScrollValue = (decimal)hScrollCursorPos / max;

                this.Refresh();
            }
            else if (e.Button == MouseButtons.Left && vScrollMouse)
            {
                vScrollCursorPos = scrollMouseOffset + e.Y;
                int max = gridHeight - vScrollCursorSize - 3;
                if (vScrollCursorPos > max) vScrollCursorPos = max;
                if (vScrollCursorPos < 0) vScrollCursorPos = 0;

                vScrollValue = (decimal)vScrollCursorPos / max;


                this.Refresh();
            }
           
        }

        private void grid_MouseLeave(object sender, EventArgs e)
        {
            if (hScrollMouse || vScrollMouse)
            {
                hScrollMouse = false;
                vScrollMouse = false;
                this.Refresh();
            }
        }

        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (hScrollMouse)
            {
                scrollMouseOffset = hScrollCursorPos - e.X;

            }
            else if (vScrollMouse)
            {
                scrollMouseOffset = vScrollCursorPos - e.Y;

            } else
            {

            }
        }

        private void grid_MouseUp(object sender, MouseEventArgs e)
        {
            if (vScrollVisible && !vScrollMouse && e.X > gridWidth && e.Y < gridHeight)
            {
                vScrollCursorPos = (int)(e.Y / vScrollCursorSize) * vScrollCursorSize;

                int max = gridHeight - vScrollCursorSize - 3;
                if (vScrollCursorPos > max) vScrollCursorPos = max;
                if (vScrollCursorPos < 0) vScrollCursorPos = 0;

                vScrollValue = (decimal)vScrollCursorPos / max;
            } else if (hScrollVisible && !hScrollMouse && e.Y > gridHeight && e.X < gridWidth)
            {
                hScrollCursorPos = (int)(e.X / hScrollCursorSize) * hScrollCursorSize;

                int max = gridWidth - hScrollCursorSize - 3;
                if (hScrollCursorPos > max) hScrollCursorPos = max;
                if (hScrollCursorPos < 0) hScrollCursorPos = 0;

                hScrollValue = (decimal)hScrollCursorPos / max;
            }
        }
    }
    public class Column
    {
        public int width;
        public string text;
        public string name;
        public int index;
    }
    public class Row
    {
        public DataRow data;
        public bool selected;

        public Row(DataRow row)
        {
            data = row;
            selected = true;
        }
    }

}
