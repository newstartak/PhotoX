using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClientAsync : MonoBehaviour, IWebSocketClientAsync
{

    public event WebSocketReceived WEB_SOCKET_RECEIVED;

    private WebSocket ws;
    private string wsUrl = "ws://localhost:1516";
    private bool isConnecting = false;
    private float reconnectDelay = 3f;
    private Coroutine reconnectCoroutine;

    [SerializeField]
    public class KisWebSocket
    {
        public KisICApproval KIS_ICApproval;
    }
    [SerializeField]
    public class KisICApproval
    {
        public string inTranCode;
        public string inTradeType;
        public string inInstallment;
        public string inTranAmt;
    }

    [SerializeField]
    public class KisReceive
    {
        public string outCatId;
        public string outReplyCode;
        public string outWCC;
        public string outCardNo;
        public string outInstallment;
        public string outTranAmt;
        public string outVatAmt;
        public string outSvcAmt;
        public string outJanAmt;
        public string outAuthNo;
        public string outReplyDate;
        public string outOrgAuthDate;
        public string outOrgAuthNo;
        public string outAccepterCode;
        public string outAccepterName;
        public string outIssuerCode;
        public string outIssuerName;
        public string outMerchantRegNo;
        public string outTranNo;
        public string outRtn;
        public string outReplyMsg1;
        public string outReplyMsg2;
        public string outAddedPoint;
        public string outUsablePoint;
        public string outTotalPoint;
        public string outShinsegaeSpec;
        public string outTradeNum;
        public string outTradeReqTime;
        public string outTradeReqDate;
        public string outVanKey;
        public string outTranGubun;
        public string outBrandGubun;
        public string outLocalCode;
        public string outLocalCodeNumber;
        public string outLocalAmount;
        public string outLocalAmountMinor;
        public string outExchangeRate;
        public string outExchangeRateMinor;
        public string outInvertedRate;
        public string outInvertedRateMinor;
        public string outInvertedRateDisplayUnit;
        public string outMarkupPercentage;
        public string outMarkupPercentageDisplayUnit;
        public string outCommissionPercentage;
        public string outCommissionPercentageMinor;
        public string outCommissionNumber;
        public string outCommissionAmount;
        public string outCommissionAmountMinor;
        public string outServiceProvider;
        public string outCommissionRate;
        public string outAccountNum;
        public string outBarCodeNumber;
        public string outReceiptYN;
        public string outAgentData;
        public string outAgentCode;
        public string outDCCAuthType;
        public string outCardType;
        public string outArrivedData;
        public string outSignHexData;
        public string outIssuerBranchCode;
        public string outAccepterBranchCode;
        public string outTokenDate;
        public string outVanCode;
        public string outNetVoidCode;
        public string outEMVData;
        public string outHashCode;
        public string outCardGubun;
        public string outPurchaseGubun;
        public string outToken;
        public string outGiftPoint;
        public string outRemainPoint;
        public string outYearUsablePoint;
        public string outEventPoint;
        public string outSalesAmt;
        public string outDiscountAmt;
        public string outSavePoint;
        public string outPaymentAmt;
        public string outPointInfo;
        public string outGreenCardPoint;
        public string outCardCompanyTradeNum;
        public string outPEM;
        public string outTRID;
        public string outMemberShipBarcode;
        public string outMerchantMemberShipNo;
        public string outPayType;
        public string outTradeType;
        public string outTranCode;
        public string outAffiliateCardInfo;
        public string outCustmerID;
        public string outReserveInfo;
        public string outUniqueNum;
        public string outStoreData;
        public string outNetCode;
        public string outReaderType;
        public string outSignPadType;
        public string outOrderNo;
        public string outPayMethod;
        public string outUserID;
        public string outMemBarcodeNo;
        public string outMemMerchantNo;
        public string outAddInfo;
        public string outFiller;
        public string outMemberNumber;
        public string outTradeID;
        public string outSignEncodeData;
        public string outPointAmt;
        public string outReceiptNumber;
        public string outGiftData;
        public string outSKTGoodsCode;
        public string outCouponInformation;
        public string outCashAmt;
        public string outPayGubun;
        public string outPointType;
        public string outRealCardNo;
        public string outEightCardNo;
        public string outTranAmtNEWPOS;
        public string outPointCardNo;
        public string outPointAuthNo;
        public string outPointIssuerCode;
        public string outPointIssuerName;
        public string outNextUploadData;
        public string outReplyMsg3;
        public string outReplyMsg4;
        public string outCatTranGubun;
        public string outAddAmt;
        public string outTaxFreeAmt;
        public string outDisplayMsg;
        public string outReplyMsg5;
        public string outPurchaseBal;
        public string outPurchaseRefundAmt;
        public string outPurchaseLimitAmt;
        public string outPrepaidCardName;
        public string outPrepaidCardStatus;
        public string outPrepaidCardType;
    }

    //싱글턴 인스턴스
    public static WebSocketClientAsync instance = null;
    //싱글턴 인스턴스 반환
    public static WebSocketClientAsync Instance
    {
        get
        {
            if (instance == null)
            {
                WebSocketClientAsync obj = FindAnyObjectByType<WebSocketClientAsync>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    WebSocketClientAsync newObj = new GameObject("WebSocketClientAsync").AddComponent<WebSocketClientAsync>();
                    instance = newObj;
                }

            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        WebSocketClientAsync[] objs = FindObjectsByType<WebSocketClientAsync>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        //UnityThread.initUnityThread();
        
    }
    bool isDestroy = false;

    public void StartDestroy()
    {
        isDestroy = true;
        
        if (reconnectThread != null && reconnectThread.IsAlive)
        {
            reconnectThread.Join();
            reconnectThread = null;
        }

        if (ws != null)
        {
            ws.OnOpen -= OnOpen;
            ws.OnMessage -= OnMessage;
            ws.OnClose -= OnClose;
            ws.OnError -= OnError;
            ws.Close();
            ws = null;
        }
    }

    public void ConnectWebSocket(string socketUrl = "ws://localhost:1516")
    {
        if (ws != null)
        {
            ws.OnOpen -= OnOpen;
            ws.OnMessage -= OnMessage;
            ws.OnClose -= OnClose;
            ws.OnError -= OnError;
            ws.Close();
            ws = null;
        }

        //isConnecting = true;
        Debug.Log("Connecting...");
        wsUrl = socketUrl;
        ws = new WebSocket(wsUrl);

        ws.OnOpen += OnOpen;
        ws.OnMessage += OnMessage;
        ws.OnClose += OnClose;
        ws.OnError += OnError;

        ws.ConnectAsync();
    }

    void OnOpen(object sender, System.EventArgs e)
    {
        /*
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log("WebSocket Connected!");
            isConnecting = true;
        });
        */


        // 예시: 연결되면 메시지 전송
        //ws.Send("Hello from Unity with WebSocketSharp!");

        NLogManager.logger.Info("WS Connected");
    }

    public void SendMessage(string inTranCode = "NV", string inTradeType = "D1", string inInstallment = "00", string inTranAmt = "1004")
    {
        if (!ws.IsAlive)
            return;
        try
        {

            //{
            //    "KIS_ICApproval":
            //    {
            //        "inTranCode":"NV","inTradeType":"D1","inInstallment":"00","inTranAmt":"1004"
            //    }
            //}
            KisWebSocket kisWebSocket = new KisWebSocket();
            kisWebSocket.KIS_ICApproval = new KisICApproval();
            kisWebSocket.KIS_ICApproval.inTranCode = inTranCode;
            kisWebSocket.KIS_ICApproval.inTradeType = inTradeType;
            kisWebSocket.KIS_ICApproval.inInstallment = inInstallment;
            kisWebSocket.KIS_ICApproval.inTranAmt = inTranAmt;

            string json = JObject.FromObject(kisWebSocket).ToString();

            //m_Socket.Send(Encoding.UTF8.GetBytes(json));
            ws.Send(json);

        }
        catch (Exception e)
        {

            Debug.Log(e.Message);
            throw;
        }

    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        //Debug.Log("Message Received: " + e.Data);

        KisReceive kisReceive = JsonUtility.FromJson<KisReceive>(e.Data);

        //outReplyCode 0000 정상승인

        WEB_SOCKET_RECEIVED?.Invoke(kisReceive.outReplyCode);

        if (kisReceive.outReplyCode == "0000")//결제 완료
        {
            NLogManager.logger.Info("Credit Card Pay Completed");

            // 인덱스 상 다음 씬은 결제 실패 화면이므로 2만큼 건너가기
            SimpleSceneManager.ChangeSceneByIndex(2);
        }
        // !!!!!! Z312, 8373 등으로 나왔었음. 재확인 필요 !!!!!!!!!!!!!!
        else if (kisReceive.outReplyCode == "8418")//제로페이
        {
            NLogManager.logger.Info("ZeroPay Completed");

            SimpleSceneManager.ChangeSceneByIndex(2);
        }
        else if (kisReceive.outReplyCode == "IC02")// 카드리딩 제한시간 초과
        {
            NLogManager.logger.Info("Card Reading Timeout");

            // 인덱스 상 다음 씬이 결제 실패 화면이므로 1만큼 건너가기
            SimpleSceneManager.ChangeSceneByIndex(1);
        }
        else if (kisReceive.outReplyCode == "IC07")// 리더기 연결 안됨
        {
            NLogManager.logger.Error("Card Reader doesn't exist");

            SimpleSceneManager.ChangeSceneByIndex(1);
        }
        else
        {
            NLogManager.logger.Warn("Unknown Purchase Method");
            NLogManager.logger.Warn("kisReceive.outTradeType=" + kisReceive.outTradeType);
            NLogManager.logger.Warn("kisReceive.outRtn=" + kisReceive.outRtn);
            NLogManager.logger.Warn("kisReceive.outReplyCode=" + kisReceive.outReplyCode);

            SimpleSceneManager.ChangeSceneByIndex(1);
        }
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log($"WebSocket Closed: code={e.Code}, reason={e.Reason}");

        isConnecting = false;
        if (!isConnecting && !isDestroy)
        {
            StartReconnectThread();
        }
        //if (!isConnecting && reconnectCoroutine == null)
        //{
        //    isConnecting = true;
        //    if (ReconnectCoroutineC != null)
        //    {
        //        StopCoroutine(ReconnectCoroutineC);
        //        ReconnectCoroutineC = null;
        //    }

        //    //reconnectCoroutine = StartCoroutine(ReconnectCoroutine());
        //    ReconnectCoroutineC = ReconnectCoroutine();
        //    StartCoroutine(ReconnectCoroutineC);
        //}
    }

    void OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log("WebSocket Error: " + e.Message);
        isConnecting = false;
        if (!isConnecting && !isDestroy)
        {
            StartReconnectThread();
        }


        //if (!isConnecting && reconnectCoroutine == null)
        //{

        //}
    }
    private Thread reconnectThread;


    void StartReconnectThread()
    {
        //isConnecting = false;
        if (reconnectThread != null && reconnectThread.IsAlive)
        {
            reconnectThread.Join(); // 종료
            reconnectThread = null;
        }

        reconnectThread = new Thread(() =>
        {
            Debug.Log($"[Thread] Waiting {reconnectDelay} seconds before reconnect...");
            Thread.Sleep(TimeSpan.FromSeconds(reconnectDelay));

            /*
            if (!isDestroy)
            {
                UnityThread.executeInUpdate(() =>
                {
                    Debug.Log("[Thread] Try to reconnect...");
                    ConnectWebSocket();
                });
                
            }
            */

        });
        reconnectThread.IsBackground = true;
        reconnectThread.Start();
    }


    void OnDestroy()
    {
        StartDestroy();
        
    }
}
