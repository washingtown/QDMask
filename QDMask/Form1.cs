using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace QDMask
{
    public partial class Form1 : Form
    {
        private IWebDriver webDriver;
        //private string mailUrl = "http://kzyynew.qingdao.gov.cn:81/dist/index.html#/preOrder";
        //private string sfUrl = "http://kzyynew.qingdao.gov.cn:81/dist/index.html#/SFOrder";
        private string mailUrl = "http://127.0.0.1:5000/preOrder";
        private string sfUrl = "http://127.0.0.1:5000/SFOrder";
        private string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)+@"\\config.ini";
        private IniFile ini = null;
        private Dictionary<string, string[]> addressDict = new Dictionary<string, string[]>
        {
            {"市南区",new string[]{"中山路街道办事处","八大关街道办事处","珠海路街道办事处","八大峡街道办事处","云南路街道办事处","湛山街道办事处","江苏路街道办事处","金门路街道办事处","八大湖街道办事处","香港中路街道办事处","金湖路街道办事处",} },
            {"市北区",new string[]{"大港街道办事处","湖岛街道办事处","水清沟街道办事处","镇江路街道办事处","海伦路街道办事处","洛阳路街道办事处","合肥路街道办事处","浮山新区街道办事处","延安路街道办事处","台东街道办事处","阜新路街道办事处","四方街道办事处","宁夏路街道办事处","敦化路街道办事处","辽源路街道办事处","河西街道办事处","即墨路街道办事处","辽宁路街道办事处","兴隆路街道办事处","双山街道办事处","登州路街道办事处",} },
            {"崂山区",new string[]{"沙子口街道办事处","金家岭街道办事处","王哥庄街道办事处","中韩街道办事处","北宅街道办事处",} },
            {"李沧区",new string[]{"世园街道办事处","楼山街道办事处","九水街道办事处","湘潭路街道办事处","兴华路街道办事处","振华路街道办事处","虎山路街道办事处","兴城路街道办事处","沧口街道办事处","李村街道办事处","浮山路街道办事处", } },
            {"青岛西海岸新区",new string[]{"青岛西海岸新区泊里镇","青岛西海岸新区薛家岛街道","青岛西海岸新区滨海街道","青岛西海岸新区黄岛街道","青岛西海岸新区灵山岛省级自然保护区","青岛西海岸新区临港经济区","青岛西海岸新区灵山卫街道","青岛西海岸新区宝山镇","青岛西海岸新区隐珠街道","青岛西海岸新区珠海街道","青岛西海岸新区藏南镇","青岛西海岸新区大村镇","青岛西海岸新区海青镇","青岛西海岸新区琅琊镇","青岛西海岸新区六汪镇","青岛西海岸新区王台镇","青岛西海岸新区张家楼镇","青岛西海岸新区红石崖街道","青岛西海岸新区长江路街道","青岛西海岸新区胶南街道办事处","青岛西海岸新区灵珠山街道","青岛西海岸新区铁山街道","青岛西海岸新区辛安街道","青岛西海岸新区大场镇",} },
            {"城阳区",new string[]{"夏庄街道办事处","棘洪滩街道办事处","惜福镇街道办事处","上马街道办事处","流亭街道办事处","城阳街道办事处","河套街道办事处","红岛街道办事处",} },
            {"即墨区",new string[]{"鳌山卫街道办事处","潮海街道办事处","龙泉街道办事处","即墨区田横镇","通济街道办事处","温泉街道办事处","环秀街道办事处","北安街道办事处","龙山街道办事处","即墨区大信镇","即墨区蓝村镇","移风店镇","即墨区金口镇","即墨区灵山镇","通济新经济区","段泊岚镇", } },
            {"平度市",new string[]{"平度市开发区管委","平度市古岘镇","平度市田庄镇","平度市新河镇","平度市仁兆镇","平度市东阁街道办事处","平度市白沙河街道办事处","平度市南村镇","平度市崔家集镇","平度市明村镇","平度市大泽山镇","平度市凤台街道办事处","平度市同和街道办事处","平度市李园街道办事处","平度市店子镇","平度市旧店镇","平度市云山镇","平度市蓼兰镇", } },
            {"莱西市",new string[]{"莱西市马连庄镇","莱西市沽河街道办事处","莱西市院上镇","莱西市夏格庄镇","莱西市望城街道办事处","莱西市日庄镇","莱西市南墅镇","莱西市店埠镇","莱西市河头店镇","莱西市经济开发区","莱西市姜山镇","莱西市水集街道办事处",} },
            {"胶州市",new string[]{"胶州市李哥庄镇","胶州市胶莱街道办事处","胶州市胶西街道办事处","胶州市里岔镇","胶州市洋河镇","胶州市铺集镇","胶州市九龙街道办事处","胶州市阜安街道办事处","胶州市中云街道办事处","胶州市三里河街道办事处","胶州市胶东街道办事处","胶州市胶北街道办事处",} },
        };
        private bool mailFilled=false;
        private bool sfFilled = false;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        ///访问链接并填写信息
        /// </summary>
        /// <param name="url"></param>
        private void fillInfo(string url)
        {
            if (webDriver.Url == url)
            {
                webDriver.Navigate().Refresh();
            }
            else
            {
                webDriver.Navigate().GoToUrl(url);
            }
            try
            {
                IWebElement okElement = webDriver.FindElement(By.ClassName("vux-check-icon"));
                okElement.Click();
                IWebElement[] inputText = webDriver.FindElements(By.ClassName("weui-input")).ToArray();
                inputText[0].SendKeys(textBoxName.Text);
                inputText[1].SendKeys(textBoxPhone.Text);
                inputText[2].SendKeys(textBoxID.Text);
                IWebElement[] selects = webDriver.FindElements(By.ClassName("weui-select")).ToArray();
                SelectElement district = new SelectElement(selects[0]);
                district.SelectByText(comboBoxDistrict.Text);
                SelectElement street = new SelectElement(selects[1]);
                street.SelectByText(comboBoxStreet.Text);
                webDriver.FindElement(By.ClassName("weui-textarea")).SendKeys(textBoxAddress.Text);
                textBoxMessage.Text = "信息已填入，快去填写验证码\r\n祝好运！";
                FlashWin();
                if (url == mailUrl)
                    mailFilled = true;
                if (url == sfUrl)
                    sfFilled = true;
                timer1.Enabled = false;
            }
            catch (NoSuchElementException)
            {
                if (webDriver.FindElement(By.TagName("body")).Text.Contains("预约已结束"))
                {
                    if (url == mailUrl)
                        mailFilled = true;
                    if (url == sfUrl)
                        sfFilled = true;
                }
                textBoxMessage.Text = "时间已到，预约还未开启，继续尝试";
                return;
            }
        }


        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FlashWindowEx")]
        private static extern void FlashWindowEx(ref FLASHWINFO pwfi);
        public struct FLASHWINFO
        {
            public UInt32 cbSize;//该结构的字节大小
            public IntPtr hwnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            public UInt32 dwFlags;//闪烁的状态
            public UInt32 uCount;//闪烁窗口的次数
            public UInt32 dwTimeout;//窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
        }
        public const UInt32 FLASHW_TRAY = 2;
        public const UInt32 FLASHW_TIMERNOFG = 12;
        private void FlashWin()
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = this.Handle;
            fInfo.dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG;
            fInfo.uCount = 3;// UInt32.MaxValue;
            fInfo.dwTimeout = 500;
            FlashWindowEx(ref fInfo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //载入区市信息
            comboBoxDistrict.Items.AddRange(this.addressDict.Keys.ToArray());
            //载入配置文件
            if (!File.Exists(iniPath))
            {
                File.CreateText(iniPath);
            }
            this.ini = new IniFile(iniPath);
            textBoxName.Text = ini.IniReadValue("info", "Name");
            textBoxPhone.Text = ini.IniReadValue("info", "Phone");
            textBoxID.Text = ini.IniReadValue("info", "ID");
            comboBoxDistrict.Text = ini.IniReadValue("info", "District");
            comboBoxStreet.Text = ini.IniReadValue("info", "Street");
            textBoxAddress.Text = ini.IniReadValue("info", "Address");
            comboBoxPortal.SelectedIndex = 0;
            //启动Chrome
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("disable-web-security");
            this.webDriver = new ChromeDriver(options);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ini.IniWriteValue("info", "Name", textBoxName.Text);
            ini.IniWriteValue("info", "Phone", textBoxPhone.Text);
            ini.IniWriteValue("info", "ID", textBoxID.Text);
            ini.IniWriteValue("info", "District", comboBoxDistrict.Text);
            ini.IniWriteValue("info", "Street", comboBoxStreet.Text);
            ini.IniWriteValue("info", "Address", textBoxAddress.Text);
            try
            {
                this.webDriver.Quit();
            }
            catch(Exception ee)
            {

            }
            try
            {
                this.webDriver.Dispose();
            }
            catch(Exception ee)
            {

            }
        }


        private void ComboBoxDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Items.Clear();
            string district = comboBoxDistrict.Text;
            if (this.addressDict.ContainsKey(district))
            {
                comboBoxStreet.Items.AddRange(this.addressDict[district]);
                comboBoxStreet.SelectedIndex = 0;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime mailStart = new DateTime(now.Year, now.Month, now.Day, 9, 30, 0);
            DateTime sfStart = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0);
            if (now < mailStart)
            {
                textBoxMessage.Text = String.Format(
                    "预约尚未开始，下次为：邮政预约，时间：{0},\r\n距离开始还有:{1}",
                    mailStart.ToString(),(mailStart-now).ToString(@"hh\:mm\:ss")
                    );
                return;
            }
            if (now > mailStart && now <sfStart && !mailFilled)
            {
                fillInfo(mailUrl);
                return;
            }
            if (now < sfStart)
            {
                textBoxMessage.Text = String.Format(
                    "预约尚未开始，下次为：顺丰预约，时间：{0},\r\n距离开始还有:{1}",
                    sfStart.ToString(), (sfStart - now).ToString(@"hh\:mm\:ss")
                    );
                return;
            }
            if(now>sfStart && !sfFilled)
            {
                fillInfo(sfUrl);
                return;
            }
            if (now > sfStart && sfFilled)
            {
                textBoxMessage.Text = "今天预约已结束，明天再试试吧！";
                return;
            }
        }
    }
}
