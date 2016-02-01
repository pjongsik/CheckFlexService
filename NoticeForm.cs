﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CheckFlexService.kr.co.mainticket.strawberry;
using System.Threading;

namespace CheckFlexService
{

    //public class CampingPlace
    //{
    //    List<CampingPlace> list = new List<CampingPlace>();

    //    public CampingPlace(string name, string code)
    //    {
    //        Name = name;
    //        Code = code;

    //        list.Add(new CampingPlace(name, code));
    //    }

    //    public string Name {get; set;}
    //    public string Code {get; set;}

    //    public string GetName(string code)
    //    {
    //        CampingPlace cp = list.Where(x => x.Code == code).FirstOrDefault();
    //        return cp == null ? string.Empty : cp.Name;
    //    }

    //}

    public partial class frmMain : Form
    {
        const string _한탄강 = "0007"; // login
        const string _용자휴 = "0009"; // login
        const string _자라섬 = "0012";
        const string _중랑숲 = "0013";
        const string _너나들 = "0045";
        const string _바라산 = "0048";
        const string _도덕산 = "0049";
        const string _ShopCode = "0001";

        public frmMain()
        {
            InitializeComponent();
        }

        FlexService _service;
        bool _continue = false;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //

           // string url = @"http://strawberry.mainticket.co.kr/C2Soft.Earth.Web.Service/FlexService.asmx";
           //// FlexService.FlexServiceSoapClient flexService = new FlexService.FlexServiceSoapClient("FlexServiceSoap", url);

           // BasicHttpBinding binding = GetEndpointAddress("FlexServiceSoap");
           // EndpointAddress endPoint = new EndpointAddress(url);
           // FlexService.FlexServiceSoapClient flexService = new FlexService.FlexServiceSoapClient(binding, endPoint);
            //FlexService.ArrayOfAnyType result = flexService.GetScheduleByGroup("0012", "0001", "201507");
            //FlexService.ArrayOfAnyType result = flexService.GetScheduleByGroup(_중랑숲, "0001", "201507");


            _service = new FlexService();


            //object[] result1 = service.GetProductGroupByBook(_자라섬, "0001");
            //foreach (var item in result1)
            //{
            //    BoardSchedule data = item as ScheduleGroup;
            //}


            // object[] result2 = service.GetProductBySeat(_중랑숲, "0001", "0001", "20150725");

            // service.
            //Thread t = new Thread(new ThreadStart(CheckCampingPlace));
            //t.Start();
          

            // 도덕산(service);
      
        }

        private void start_Click(object sender, EventArgs e)
        {
            CheckStart();
        }

        private void CheckStart()
        {
            _continue = true;


            //
            Thread t = new Thread(new ThreadStart(CheckCampingPlace));
            t.Start();

            start.Enabled = false;
            stop.Enabled = true;

            //
            startitem.Enabled = false;
            stopItem.Enabled = true;
        }


        private void stop_Click(object sender, EventArgs e)
        {
            CheckStop();
        }

        private void CheckStop()
        {
            _continue = false;

            start.Enabled = true;
            stop.Enabled = false;

            //
            startitem.Enabled = true;
            stopItem.Enabled = false;
            
        }


        void CheckCampingPlace()
        {
            while (_continue)
            {

                //CheckReservationable(_service, _도덕산, _ShopCode, new List<string>() { "20150822", "20150829", "20150905" });
                CheckReservationable(_service, _중랑숲, _ShopCode, new List<string>() { "20151017", "20151031" });
                CheckReservationable(_service, _한탄강, _ShopCode, new List<string>() { "20151024" }, new List<string>() { "캐빈" });
                CheckReservationable(_service, _한탄강, _ShopCode, new List<string>() { "20151017", "20151031" }, new List<string>() {"자동차"});
                CheckReservationable(_service, _자라섬, _ShopCode, new List<string>() { "20151017", "20151031" }, new List<string>() {"캐라반사이트B"});
                CheckReservationable(_service, _도덕산, _ShopCode, new List<string>() { "20151010", "20151017", "20151031" });


                Thread.Sleep(3000);
            }
           
        }

        bool _reservationable = false;
        int _checkCount = 0;
        /// <summary>
        /// 체크
        /// </summary>
        /// <param name="service"></param>
        /// <param name="campingPlace"></param>
        /// <param name="shopCode"></param>
        /// <param name="checkDateList"></param>
        public void CheckReservationable(FlexService service, string campingPlace, string shopCode, List<string> checkDateList, List<string> checkGroupNameList = null)
        {
            bool validateTrue = false;

            string url = @"https://strawberry.mainticket.co.kr/strawberry/strawberry.aspx?companycode={0}&shopcode={1}";

            object[] baseData = service.GetProductByBook(campingPlace, shopCode);

            List<string> checkMonthList = checkDateList.GroupBy(x => x.Substring(0, 6)).Select(x => x.Key).OrderBy(x => x).ToList();

            foreach (string month in checkMonthList)
            {
                // 해당 월 전체 정보
                object[] result = service.GetScheduleByGroup(campingPlace, _ShopCode, month);

                foreach (var item in result)
                {
                    ScheduleGroup data = item as ScheduleGroup;

                    List<string> checkDates = checkDateList.Where(x => x.StartsWith(month)).ToList();

                    if (checkDates.Contains(data.PlayDate))
                    {
                        if (checkGroupNameList == null || CheckGroupName(checkGroupNameList, data.GroupName))
                        {
                            string text = string.Format("[{7}] - GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}"
                                , data.GroupCode, data.GroupKind, data.GroupName, data.MaxSaleCount, data.PlayDate, data.PlayDays, data.ProductRemainCount, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + "\r\n";

                            DisplayText(text);

                            if (data.ProductRemainCount > 0 && _reservationable == false)
                            {

                                ShowAlertForm(string.Format("{0}, {1}, {2}", campingPlace, data.GroupName, data.PlayDate));

                                validateTrue = true;


                                DisplayText("\r\n" + "\r\n" + "\r\n" + " ==============> 예약가능 <====================" + "\r\n");
                                DisplayText(" ==============> 예약가능 <====================" + "\r\n");
                                DisplayText(" ==============> 예약가능 <====================" + "\r\n" + "\r\n" + "\r\n" + "\r\n");

                                System.Diagnostics.Process.Start(string.Format(url, campingPlace, _ShopCode));
                            }
                        }
                    }
                }

                _checkCount++;

                if (_checkCount > 600)
                {
                    CrearDisplayText();
                    _checkCount = 0;
                }
            }

            /// 한건이라도 예약가능한 건이 있으면 TRUE 
            
            _reservationable = validateTrue;
        }

        bool CheckGroupName(List<string> groupNameList, string checkName)
        {
            if (groupNameList == null) return true;
            else
            {
                foreach (var name in groupNameList)
                {
                    if (checkName.StartsWith(name)) return true;
                }

                return false;
            }
        }

        void ShowAlertForm(string text)
        {
            using (AlertForm aForm = new AlertForm(text))
            {
                if (aForm.InvokeRequired)
                {
                    aForm.BeginInvoke(new Action(() => aForm.ShowDialog()));
                }
                else
                {
                    aForm.ShowDialog();
                }
            }
        }

        void CrearDisplayText()
        {
            if (txtBox.InvokeRequired)
            {
                txtBox.BeginInvoke(new Action(() => txtBox.Clear()));
            }
            else
            {
                txtBox.Clear();
            }
        }

        void DisplayText(string text)
        {
            if (txtBox.InvokeRequired)
            {
                txtBox.BeginInvoke(new Action(() => txtBox.Text += text ));
                txtBox.BeginInvoke(new Action(() => txtBox.SelectionStart = txtBox.Text.Length));
                txtBox.BeginInvoke(new Action(() => txtBox.ScrollToCaret()));
            }
            else
            {
                txtBox.Text += text;
                txtBox.SelectionStart = txtBox.Text.Length;
                txtBox.ScrollToCaret();
            }
        }

        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //base.OnFormClosing(e);
            e.Cancel = true;

            this.Hide();

            notifyIcon1.Visible = true;

            //notifyIcon1.ShowBalloonTip(100);

        }

         private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal; // 최소화를 멈춘다 

            this.Activate(); // 폼을 활성화 시킨다

            this.notifyIcon1.Visible = false;


            //this.WindowState = FormWindowState.Normal;
            //notifyIcon1.Visible = false;
        }

     
        private void button1_Click(object sender, EventArgs e)
        {
            ProcessExit();
        }

        private void ProcessExit()
        {
            notifyIcon1.Visible = false;

            this.Dispose();

            Application.Exit();
            Application.ExitThread();
            Environment.Exit(0);
        }

        # region 테스트 코드
        void 중랑숲(FlexService service)
        {

            object[] baseData = service.GetProductByBook(_중랑숲, _ShopCode);


            // 특정 사이트 정보
            //object[] result44 = service.GetScheduleByBook(_도덕산, _ShopCode, productCodeB44);
            //foreach (var item in result44)
            //{
            //    Schedule data = item as Schedule;

            //    txtBox.Text += string.Format("PlayDate:{0}, RemainCount:{1}"
            //            , data.PlayDate, data.RemainCount) + "\r\n";
            //}

            // 해당 월 전체 정보
            object[] result = service.GetScheduleByGroup(_도덕산, _ShopCode, "201508");

            foreach (var item in result)
            {
                ScheduleGroup data = item as ScheduleGroup;


                if (data.PlayDate == "20150822" || data.PlayDate == "20150829")
                {
                    txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}"
                        , data.GroupCode, data.GroupKind, data.GroupName, data.MaxSaleCount, data.PlayDate, data.PlayDays, data.ProductRemainCount) + "\r\n";

                }
            }

            //// 일자별 그룹정보
            //object[] result1 = service.GetScheduleByGroupMap(_도덕산, _ShopCode, "0001", "20150822", "20150822");


            //foreach (var item in result1)
            //{
            //    ProductMap data = item as ProductMap;

            //    txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}, RemainCount:{7}, StatusCode: {8}"
            //            , data.GroupCode, data.OptionAmount, data.OptionFlag, data.OptionName, data.OptionYn, data.ProductCode, data.ProductName, data.RemainCount, data.StatusCode) + "\r\n";
            //}

        }


        void 도덕산(FlexService service)
        {

            object[] baseData = service.GetProductByBook(_도덕산, _ShopCode);


            // 특정 사이트 정보
            //object[] result44 = service.GetScheduleByBook(_도덕산, _ShopCode, productCodeB44);
            //foreach (var item in result44)
            //{
            //    Schedule data = item as Schedule;

            //    txtBox.Text += string.Format("PlayDate:{0}, RemainCount:{1}"
            //            , data.PlayDate, data.RemainCount) + "\r\n";
            //}

            // 해당 월 전체 정보
            object[] result = service.GetScheduleByGroup(_도덕산, _ShopCode, "201508");

            foreach (var item in result)
            {
                ScheduleGroup data = item as ScheduleGroup;


                if (data.PlayDate == "20150822" || data.PlayDate == "20150829")
                {
                    txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}"
                        , data.GroupCode, data.GroupKind, data.GroupName, data.MaxSaleCount, data.PlayDate, data.PlayDays, data.ProductRemainCount) + "\r\n";

                }
            }

            //// 일자별 그룹정보
            //object[] result1 = service.GetScheduleByGroupMap(_도덕산, _ShopCode, "0001", "20150822", "20150822");


            //foreach (var item in result1)
            //{
            //    ProductMap data = item as ProductMap;

            //    txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}, RemainCount:{7}, StatusCode: {8}"
            //            , data.GroupCode, data.OptionAmount, data.OptionFlag, data.OptionName, data.OptionYn, data.ProductCode, data.ProductName, data.RemainCount, data.StatusCode) + "\r\n";
            //}

        }


        void 자라섬(FlexService service)
        {
            // B사이트 44
            string productCodeB44 = "00003044";
            string productCodeB46 = "00003046";


            //object[] baseData = service.GetBaseData(_자라섬, _ShopCode);
            object[] baseData = service.GetProductByBook(_자라섬, _ShopCode);


            // 특정 사이트 정보
            object[] result44 = service.GetScheduleByBook(_자라섬, _ShopCode, productCodeB44);
            foreach (var item in result44)
            {
                Schedule data = item as Schedule;

                txtBox.Text += string.Format("PlayDate:{0}, RemainCount:{1}"
                        , data.PlayDate, data.RemainCount) + "\r\n";
            }

            // 해당 월 전체 정보
            object[] result = service.GetScheduleByGroup(_자라섬, _ShopCode, "201508");


            // 일자별 그룹정보
            object[] result1 = service.GetScheduleByGroupMap(_도덕산, _ShopCode, "0001", "20150822", "20150822");


            foreach (var item in result)
            {
                ScheduleGroup data = item as ScheduleGroup;

                txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}"
                        , data.GroupCode, data.GroupKind, data.GroupName, data.MaxSaleCount, data.PlayDate, data.PlayDays, data.ProductRemainCount) + "\r\n";
            }


            foreach (var item in result1)
            {
                ProductMap data = item as ProductMap;

                txtBox.Text += string.Format("GroupCode:{0}, GroupKind:{1}, GroupName:{2}, MaxSaleCount:{3}, PlayDate:{4}, PlayDays:{5}, ProductRemainCount:{6}, RemainCount:{7}, StatusCode: {8}"
                        , data.GroupCode, data.OptionAmount, data.OptionFlag, data.OptionName, data.OptionYn, data.ProductCode, data.ProductName, data.RemainCount, data.StatusCode) + "\r\n";
            }

        }

        #endregion

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckStop();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckStart();
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessExit();
        }

     

       
       
      
        

       
    }
}
