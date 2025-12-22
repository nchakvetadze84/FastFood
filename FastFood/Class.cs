using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using FastFood.Properties;
using System.Resources;
using System.IO;
using System.Xml;
using System.Globalization;

namespace FastFood
{

    public class FFLinkLabel : System.Windows.Forms.LinkLabel
    {
        private Order m_Order = null;

        public Order Order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }
    }

    public class FFbutton : System.Windows.Forms.Button
    {
        private Menu m_Menu = null;

        public Menu Menu
        {
            get { return m_Menu; }
            set { m_Menu = value; }
        }

        public bool IsBusy { set; get; }
    }
    public class Menu
    {
        private int m_Id = 0;
        private string m_Name = String.Empty;
        private string m_NameEn = String.Empty;
        private Image m_Picture = null;
        private decimal m_price = 0;
        private int m_Category_ID = 0;
        private int m_Type = 0;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string NameEn
        {
            get { return m_NameEn; }
            set { m_NameEn = value; }
        }

        public Image Picture
        {
            get { return m_Picture; }
            set { m_Picture = value; }
        }

        public decimal Price
        {
            get { return m_price; }
            set { m_price = value; }
        }
        public int Category
        {
            get { return m_Category_ID; }
            set { m_Category_ID = value; }
        }

        public int Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public Menu()
        {

        }

        public Menu(int menuID)
        {
            DataTable table = DBObjectNew.InvokeTString("SELECT [ID], [Name_GE], [Name_EN], [Price], [Status], [Categorie_ID], [Menu_Type_ID] FROM [dbo].[Menu] where [ID] = " + menuID.ToString());
            m_Id = menuID;
            m_Name = table.Rows[0]["Name_GE"].ToString();
            m_NameEn = table.Rows[0]["Name_EN"].ToString();
            m_price = Convert.ToDecimal(table.Rows[0]["Price"]);
            m_Category_ID = Convert.ToInt32(table.Rows[0]["Categorie_ID"]);
            m_Type = Convert.ToInt32(table.Rows[0]["Menu_Type_ID"]);
        }

        public Menu(int Id, string name, string nameEn, Image picture, decimal price, int category_id, int type_id)
        {
            m_Id = Id;
            m_Name = name;
            m_NameEn = nameEn;
            m_Picture = picture;
            m_price = price;
            m_Category_ID = category_id;
            m_Type = type_id;
        }
    }

    public class Order
    {
        private int m_ID = 0;

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        private string m_CheckNo = String.Empty;

        public int TableID { set; get; }
        public string TableName { set; get; }

        private List<OrderItem> m_OrderList = new List<OrderItem>();
        private List<OrderItem> m_PrevOrderList = new List<OrderItem>();
        private List<OrderItem> m_RemovedOrderList = new List<OrderItem>();

        public List<OrderItem> OrderList
        {
            get { return m_OrderList; }
            set { m_OrderList = value; }
        }

        public List<OrderItem> PrevOrderList
        {
            get { return m_PrevOrderList; }
            set { m_PrevOrderList = value; }
        }

        public List<OrderItem> RemovedOrderList
        {
            get { return m_RemovedOrderList; }
            set { m_RemovedOrderList = value; }
        }

        public string CheckNo
        {
            get { return m_CheckNo; }
            set { m_CheckNo = value; }
        }

        public static Order Clone(Order order)
        {
            Order o = new Order();
            if (order != null)
            {
                o.CheckNo = order.CheckNo;
                o.m_ID = order.ID;
                o.TableID = order.TableID;
                o.TableName = order.TableName;
                o.m_OrderList = new List<OrderItem>();//order.OrderList;
                o.m_OrderList.AddRange(order.OrderList);
                o.m_PrevOrderList = new List<OrderItem>(); //order.PrevOrderList;
                o.m_PrevOrderList.AddRange(order.PrevOrderList);
                o.m_RemovedOrderList = new List<OrderItem>();//order.m_RemovedOrderList;
                o.m_RemovedOrderList.AddRange(order.m_RemovedOrderList);
            }
            return o;
        }

        public List<OrderItem> GetOrderListByMenuType(int kitchen)
        {
            List<OrderItem> retlist = new List<OrderItem>();
            foreach (OrderItem oi in m_OrderList)
            {
                if (oi.Menu.Type == kitchen)
                {
                    if (GetOrderItemByMenuID(oi.Menu.Id, m_PrevOrderList) == null)
                    {
                        OrderItem noi = new OrderItem();
                        noi.Menu = oi.Menu;
                        noi.Quantity = oi.Quantity;
                        retlist.Add(noi);
                    }
                    if ((GetOrderItemByMenuID(oi.Menu.Id, m_PrevOrderList) != null && GetOrderItemByMenuID(oi.Menu.Id, m_PrevOrderList).Quantity < oi.Quantity))
                    {
                        OrderItem noi = new OrderItem();
                        noi.Menu = oi.Menu;
                        noi.Quantity = oi.Quantity - GetOrderItemByMenuID(oi.Menu.Id, m_PrevOrderList).Quantity;
                        retlist.Add(noi);

                        //oi.Quantity = oi.Quantity - GetOrderItemByMenuID(oi.Menu.Id, m_PrevOrderList).Quantity;
                        //retlist.Add(noi);
                    }
                }
            }
            return retlist;
        }

        public bool Validate()
        {
            bool flag = true;
            foreach (OrderItem prevoi in m_PrevOrderList)
            {
                if (GetOrderItemByMenuID(prevoi.Menu.Id, m_OrderList) == null)// || (GetOrderItemByMenuID(prevoi.Menu.Id, PrevOrderList) != null) && GetOrderItemByMenuID(prevoi.Menu.Id, PrevOrderList).Quantity > prevoi.Quantity)
                {
                    m_RemovedOrderList.Add(prevoi);
                    flag = false;
                }
                if (GetOrderItemByMenuID(prevoi.Menu.Id, m_OrderList) != null && GetOrderItemByMenuID(prevoi.Menu.Id, m_OrderList).Quantity < prevoi.Quantity)
                {
                    OrderItem oi = new OrderItem();
                    oi.Menu = prevoi.Menu;
                    oi.Quantity = prevoi.Quantity - GetOrderItemByMenuID(prevoi.Menu.Id, m_OrderList).Quantity;
                    m_RemovedOrderList.Add(oi);
                    flag = false;
                }
            }
            return flag;
        }

        private static OrderItem GetOrderItemByMenuID(int id, List<OrderItem> orderList)
        {
            foreach (OrderItem oi in orderList)
            {
                if (oi.Menu.Id == id)
                    return oi;
            }
            return null;
        }

        public decimal Sum()
        {
            decimal sum = 0;
            for (int i = 0; i < OrderList.Count; i++)
            {
                sum += m_OrderList[i].Quantity * m_OrderList[i].Menu.Price;
            }
            return sum;
        }

        public void Clear()
        {
            m_CheckNo = String.Empty;
            m_OrderList = new List<OrderItem>();
            m_PrevOrderList = new List<OrderItem>();
            m_RemovedOrderList = new List<OrderItem>();
        }

        public Order()
        {
        }
    }

    public class OrderItem
    {
        private Menu m_Menu = new Menu();
        private int m_Quantity = 0;

        public Menu Menu
        {
            get { return m_Menu; }
            set { m_Menu = value; }
        }
        public int Quantity
        {
            get { return m_Quantity; }
            set { m_Quantity = value; }
        }

        public OrderItem()
        {
        }

        //public OrderItem(int menuID)
        //{

        //}
    }

    public class Translate
    {
        private static XmlDocument Source
        {
            get
            {
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

                //FileInfo fi = new FileInfo(filePath + "Translate.xml");

                FileStream fs = new FileStream(filePath + "Translate.xml", FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs, System.Text.Encoding.Default);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader.ReadToEnd());

                return doc;
            }
        }

        public static LanguageClass Load(string language)
        {
            XmlDocument doc = Source;

            XmlNode myNode = doc.GetElementsByTagName("Translate")[0];
            LanguageClass languageC = new LanguageClass();
            try
            {
                foreach (XmlNode node in myNode.ChildNodes)
                {
                    languageC[node.Name] = node.InnerText.Split(';')[language == "ka" ? 0 : 1];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error translate");
            }
            return languageC;
        }
    }

    public class LanguageClass
    {
        private Dictionary<String, String> _phrases = new Dictionary<string, string>();

        public String this[String key]
        {
            get
            {
                if (_phrases.ContainsKey(key))
                    return _phrases[key];
                else
                    return key;
            }
            set
            {
                _phrases.Add(key, value);
            }
        }
    }

    public static class Globals
    {
        private static string m_Language = String.Empty;

        public static string Language
        {
            get { return Globals.m_Language; }
            set { Globals.m_Language = value; }
        }


        public static void LoadData()
        {
            //lcGE = Translate.Load("ka");
            //lcEN = Translate.Load("en");
        }

        private static LanguageClass lcGE = new LanguageClass();
        private static LanguageClass lcEN = new LanguageClass();

        public static string GetString(string s)
        {
            return Language == "ka" ? lcGE[s] : lcEN[s];
        }
    }

    public abstract class DataGridViewImageButtonCell : DataGridViewButtonCell
    {
        private bool _enabled;                // Is the button enabled
        private PushButtonState _buttonState; // What is the button state
        protected Image _buttonImageHot;      // The hot image
        protected Image _buttonImageNormal;   // The normal image
        protected Image _buttonImageDisabled; // The disabled image
        private int _buttonImageOffset;       // The amount of offset or border around the image

        protected DataGridViewImageButtonCell()
        {
            // In my project, buttons are disabled by default
            _enabled = true;
            _buttonState = PushButtonState.Normal;

            // Changing this value affects the appearance of the image on the button.
            _buttonImageOffset = 2;

            // Call the routine to load the images specific to a column.
            LoadImages();
        }

        // Button Enabled Property
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                _buttonState = value ? PushButtonState.Normal : PushButtonState.Disabled;
            }
        }

        // PushButton State Property
        public PushButtonState ButtonState
        {
            get { return _buttonState; }
            set { _buttonState = value; }
        }

        // Image Property
        // Returns the correct image based on the control's state.
        public Image ButtonImage
        {
            get
            {
                switch (_buttonState)
                {
                    case PushButtonState.Disabled:
                        return _buttonImageDisabled;

                    case PushButtonState.Hot:
                        return _buttonImageHot;

                    case PushButtonState.Normal:
                        return _buttonImageNormal;

                    case PushButtonState.Pressed:
                        return _buttonImageNormal;

                    case PushButtonState.Default:
                        return _buttonImageNormal;

                    default:
                        return _buttonImageNormal;
                }
            }
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            // Draw the cell background, if specified.
            if ((paintParts & DataGridViewPaintParts.Background) ==
                DataGridViewPaintParts.Background)
            {
                SolidBrush cellBackground =
                    new SolidBrush(cellStyle.BackColor);
                graphics.FillRectangle(cellBackground, cellBounds);
                cellBackground.Dispose();
            }

            // Draw the cell borders, if specified.
            if ((paintParts & DataGridViewPaintParts.Border) ==
                DataGridViewPaintParts.Border)
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                    advancedBorderStyle);
            }

            // Calculate the area in which to draw the button.
            // Adjusting the following algorithm and values affects
            // how the image will appear on the button.
            Rectangle buttonArea = cellBounds;

            Rectangle buttonAdjustment =
                BorderWidths(advancedBorderStyle);

            buttonArea.X += buttonAdjustment.X;
            buttonArea.Y += buttonAdjustment.Y;
            buttonArea.Height -= buttonAdjustment.Height;
            buttonArea.Width -= buttonAdjustment.Width;

            Rectangle imageArea = new Rectangle(
                buttonArea.X + _buttonImageOffset,
                buttonArea.Y + _buttonImageOffset,
                16,
                16);

            ButtonRenderer.DrawButton(graphics, buttonArea, ButtonImage, imageArea, false, ButtonState);
        }

        // An abstract method that must be created in each derived class.
        // The images in the derived class will be loaded here.
        public abstract void LoadImages();
    }

    // Create a column class to display the Save buttons.
    public class DataGridViewImageButtonDeleteColumn : DataGridViewButtonColumn
    {
        public DataGridViewImageButtonDeleteColumn()
        {
            this.CellTemplate = new DataGridViewImageButtonSaveCell();
            this.Width = 22;
            this.Resizable = DataGridViewTriState.False;
        }
    }

    // implemented is LoadImages to load the Normal, Hot and Disabled Save images.
    public class DataGridViewImageButtonSaveCell : DataGridViewImageButtonCell
    {
        public override void LoadImages()
        {
            _buttonImageHot = Resources.cancel;
            _buttonImageNormal = Resources.cancel;
            _buttonImageDisabled = Resources.cancel;
        }
    }
}
