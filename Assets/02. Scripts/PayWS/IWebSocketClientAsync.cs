using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WebSocketClientAsync;

public delegate void WebSocketReceived(string outReplyCode);
interface IWebSocketClientAsync
{
    event WebSocketReceived WEB_SOCKET_RECEIVED;
    /// <summary>
    /// 접속
    /// </summary>
    /// <param name="socketUrl">웹소켓 아이피 ws://localhost:1516</param>
    void ConnectWebSocket(string socketUrl = "ws://localhost:1516");


    /// <summary>
    /// 카드 리더기로 프로토콜 전송 (문서 참고)
    /// </summary>
    /// <param name="inTranCode">NV:신용 및 현금영수증 거래</param>
    /// <param name="inTradeType">거래구분 코드 D1 신용승인</param>
    /// <param name="inInstallment">할부개월 00</param>
    /// <param name="inTranAmt">결제금액 테스트 금액은 1004원</param>
    void SendMessage(string inTranCode = "NV", string inTradeType = "D1", string inInstallment = "00", string inTranAmt = "1004");

    /// <summary>
    /// 삭제
    /// </summary>
    void StartDestroy();
}
