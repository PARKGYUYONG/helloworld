using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SOFD.Properties;
using SOFD.InterfaceTimeout;
using SECP.Library;
using SOFD.Component;
using SOFD.Component.Interface;

using SOFD.Logger;
using SOFD.Global.Manager;
using System.Threading;
using SECP.Library.Property;
using SOFD.Control;
using Driver_EQP_PASSIVE;
using SOFD.Global.Interface;
using SECP.Library.Manager;

namespace SECP.Biz.MainAssy.Programs
{

    public class CBMSAssyAPDReport : AProgram
    {
        private CMain _main = null;
        private bool _enable = false;

        private CWorkZoneControl _BMS_Assy_WZControl = null;

        public CBMSAssyAPDReport()
        {
        }

        public override int Init()
        {
            _enable = true;
            _BMS_Assy_WZControl.ProgramsAdd(this);

            return 0;
        }

        public override int AddArgs(object[] args, bool beforeClear)
        {
            if (args == null || args.Length < 2)
                return -1;


            if (!(args[0] is CMain) || args[1] == null)
            {
                return -1;
            }

            if (beforeClear)
            {

            }

            _main = args[0] as CMain;
            _BMS_Assy_WZControl = args[1] as CWorkZoneControl;
            return 0;
        }
        public override string Name
        {
            //get { return "BMS_ASSY_APD_REPORT"; }
            get { return "APD_REPORT"; }
        }

        public override string Version
        {
            get { return "ver 1.0"; }
        }

        public override string Description
        {
            get { return "BMS Assy 판정 실처리 데이터 보고1111222"; }
        }
        public override bool Enable
        {
            get { return _enable; }
        }
        public override string Development
        {
            get { return "(주)서진정보기술-박규용K 2019-03-21";}//-광저우CO
        }

        public override bool IsCycle
        {
            get { return false; }
        }
        public override string SIteName
        {
            get { return "VINFAST_BA"; }
        }
        public override bool IsAsyncCall
        {
            get { return false; }
        }

        private const int T1 = 4000;
        private const double T2 = 4000;

        protected override int InnerExecute()
        {
            Thread.Sleep(500);
            string errorType = "";
            ACommand command = null;

            bool noResponse = false;
            bool isTimeout = false;
            bool isOffline = false;
            string id = "";
            string rcvYNFlag = "0";   

            string strJigId = "";         
            string strResultCode = "";
            string strBoltCnt = "";
            string strBMSID_A = "";
            string strBMSID_B = "";
            string strBMSID_Sum = "";
            int count = 0;
         
            this.StatusChange(enumProgramStatus.PROCESSING);

            this.Log("STEP00");
            List<string> apdData = new List<string>();

            apdData = new List<string>(_BMS_Assy_WZControl.IW_APD.Split(' '));

            strJigId = apdData[0].Trim('\0');
            strResultCode = apdData[1].Trim('\0');
            strBoltCnt = apdData[2].Trim('\0'); // 영역 확인 필요함

            string tempA = "";
            for (int i = 16; i < 32; i++)
            {
                tempA += SmartDevice.UTILS.PLCUtils.HexToAscii(SmartDevice.UTILS.PLCUtils.DecToHex(apdData[i])).Trim();
            }
            strBMSID_A = tempA.Trim('\0');

            string tempB = "";
            for (int i = 32; i < 48; i++)
            {
                tempB += SmartDevice.UTILS.PLCUtils.HexToAscii(SmartDevice.UTILS.PLCUtils.DecToHex(apdData[i])).Trim();
            }
            strBMSID_B = tempB.Trim('\0');

            if (strBMSID_A != "")
            {
                if (strBMSID_B != "")
                {
                    strBMSID_Sum = strBMSID_A + "@" + strBMSID_B;
                }
                else
                {
                    strBMSID_Sum = strBMSID_A;
                }             
            }           
            else
            {
                if (strBMSID_B != "")
                {
                    strBMSID_Sum = strBMSID_B;
                }
            }

            //CTimeout timeout = CTimeoutManager.GetTimeout(this.ControlName, T1);
            //timeout.TargetOffValueCheck = true;
            //timeout.Begin(_BMS_Assy_WZControl._OB_WORK_COMPLETE_APD_CONFIRM, _BMS_Assy_WZControl.__IB_WORK_COMPLETE_APD as ITimeoutResource);

            //_BMS_Assy_WZControl.OB_WORK_COMPLETE_APD_CONFIRM = true;

            //if (!CTimeout.WaitSync(timeout, 10))
            //{

            //}

            //_BMS_Assy_WZControl.OB_WORK_COMPLETE_APD_CONFIRM = false;
            this.Log("STEP01 JIGID:" + strJigId + " JUDGE:" + strResultCode + " BoltCnt:" + strBoltCnt + " BMSID_A:" + strBMSID_A + " BMSID_B:" + strBMSID_B);
            if (_main.Hsms1.CommunicationsState == Library.Manager.enumCommunicationsState.ENABLED_COMMUNICATING && _main.Hsms1.ControlState == Library.Manager.enumControlState.ONLINE_REMOTE)
            {
                //_main.Hsms1.BMSAssyAPDReport.RcvFlag = false;
                _main.Hsms1.BMSAssyAPDReportRCMD.RcvFlag = false;
                this.Log("STEP02");

                //_main.Hsms1.GetVIDItem("103").Value = DateTime.Now.ToString("yyyyMMddHHmmss");//103	Clock
                //_main.Hsms1.GetVIDItem("110").Value = _BMS_Assy_WZControl.UnitNo;//110 Unit ID
                //_main.Hsms1.GetVIDItem("315").Value = strJigId;//315 JIGID    
                //_main.Hsms1.GetVIDItem("350").Value = strBMSID_Sum;//350 BMSID             
                //_main.Hsms1.GetVIDItem("375").Value = strResultCode;//375 Judge         
                //_main.Hsms1.GetVIDItem("388").Value = strBoltCnt;//388 Bolt Count   

                //_main.Hsms1.S6F11_SendMessage(Library.Manager.CHSMSEQPManager.enumCEID.ID1100_BMS_Ass_y_APD_Report);

                string clock = DateTime.Now.ToString("yyyyMMddHHmmss");
                string unitID = _BMS_Assy_WZControl.UnitNo.ToString();

                _main.Hsms1.S6F11_SendID1100(Library.Manager.CHSMSEQPManager.enumCEID.ID1100_BMS_Ass_y_APD_Report, clock, unitID, strJigId, strBMSID_Sum, strResultCode, strBoltCnt);

                bool waitCmd = CSystemConfig.GetOption("WAIT_HOST_CMD", bool.TrueString, new List<string>() { "True", "False" }).GetBoolValue1();
                while (!_main.Hsms1.BMSAssyAPDReportRCMD.RcvFlag && (count++ < 4000 || waitCmd))
                {
                    Thread.Sleep(10);
                }

                noResponse = !_main.Hsms1.BMSAssyAPDReportRCMD.RcvFlag;
                isTimeout = noResponse;

                if (_main.Hsms1.BMSAssyAPDReportRCMD.RcvFlag)
                {
                    _main.Hsms1.BMSAssyAPDReportRCMD.RcvFlag = false;
                    rcvYNFlag = _main.Hsms1.BMSAssyAPDReportRCMD.GetPLCYNFlag();
                }

                //bool waitCmd = CSystemConfig.GetOption("WAIT_HOST_CMD", bool.TrueString, new List<string>() { "True", "False" }).GetBoolValue1();
                //while (!_main.Hsms1.BMSAssyAPDReport.RcvFlag && (count++ < 4000 || waitCmd))
                //{
                //    Thread.Sleep(10);
                //}
                //noResponse = !_main.Hsms1.BMSAssyAPDReport.RcvFlag;
                //isTimeout = noResponse;    

            }
            else
            {
                this.Log("STEP03");
                isOffline = true;
                noResponse = true;
            }

            if(noResponse)
            {
                string reason = "";
                if (isOffline)
                {
                    id = "E0";//OFFLINE
                    reason = "MES OFFLINE";
                }
                else if (isTimeout)
                {
                    id = "E1";//TIMEOUT
                    reason = "MES TIMEOUT";
                }
                else
                {
                    id = "E9";//UNKNOWN
                    reason = "MES UNKNOWN";
                }

                this.Log("STEP04 NO RESPONSE PGM106_01 SET ID=" + id);

                //CUIManager.ShowMsgForm(msgId, "", message, enumButtonType.RETRY | enumButtonType.OK, new List<string>() { "REQUEST:LM_DATA_REQ", "" });

                command = CUIManager.Inst.GetCommand("Form");
                command.SetSubCommand("ShowMessageForm");
                command.AddParameter("MSG_NO", "PGM00002");
                command.AddParameter("ERR_MSG", "UNIT NO : " + _BMS_Assy_WZControl.UnitNo + " BMS_ASSY_APD_REPORT " + reason + " ERR <" + id + ">");
                command.AddParameter("MSG", "UNIT NO : " + _BMS_Assy_WZControl.UnitNo + " BMS_ASSY_APD_REPORT " + reason + " ERR <" + id + ">");
                command.AddParameter("TAG", string.Format("{0},{1},{2}", this.Name, T2, id));
                command.AddParameter("BUTTON_TYPE", "OK");
                command.AddParameter("RETURN_CMD", "");
                command.Execute();

                CBasicControl component = null;
                Dictionary<string, string> messageValues = new Dictionary<string, string>();

                component = _main.GetComponent("WORK_CV04");
                messageValues.Add("MESSAGE", reason);
                component.GetProgram("EQP_MESSAGE_DISPLAY").Execute(messageValues);
            }

            CTimeout timeout = CTimeoutManager.GetTimeout(this.ControlName, T1);
            timeout.TargetOffValueCheck = true;
            timeout.Begin(_BMS_Assy_WZControl._OB_WORK_COMPLETE_APD_CONFIRM, _BMS_Assy_WZControl.__IB_WORK_COMPLETE_APD as ITimeoutResource);

            bool plcTest = CSystemConfig.GetOption("PLC_TEST_FLAG", bool.FalseString, new List<string>() { "True", "False" }).GetBoolValue1();

            if ((!isOffline && !isTimeout) || (plcTest && isOffline))
            {
                _BMS_Assy_WZControl.OB_WORK_COMPLETE_APD_CONFIRM = true;

                if (!CTimeout.WaitSync(timeout, 10))
                {

                }
                _BMS_Assy_WZControl.OB_WORK_COMPLETE_APD_CONFIRM = false;

                if (errorType == "")
                    return 0;
                this.Log("STEP07");
                #region 메시지 창 표시

                //CUIManager.ShowMsgForm(msgId, "", message, enumButtonType.RETRY | enumButtonType.OK, new List<string>() { "REQUEST:LM_DATA_REQ", "" });

                //command = CUIManager.Inst.GetCommand("Form");
                //command.SetSubCommand("ShowMessageForm");
                //command.AddParameter("MSG_NO", "PGM00001");
                //command.AddParameter("TAG", string.Format("{0},{1},{2}", this.Name, T1, id));
                //command.AddParameter("BUTTON_TYPE", "OK");
                //command.AddParameter("RETURN_CMD", ",REQUEST:BMS_ASSY_APD_REPORT");
                //command.Execute();

                #endregion
            }
            else
            {

            }

            return -1;
        }

        public override int ExecuteManual(Dictionary<string, string> values)
        {
            _values = values;
            return InnerExecute();
        }
    }
}
