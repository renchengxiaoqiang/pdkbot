fetch('https://mms.pinduoduo.com/chats/userinfo/realtime?get_response=true').then(res => {
    res.json().then(r => {
        chrome.webview.postMessage({
            type: 'currentuser', response: JSON.stringify({
                userName: r.username,
                mallName: r.mall.mall_name,
                userId: r.id.toString(),
                mallId: r.mall.mall_id.toString(),
                avatar: r.mall.logo
            })
        })
    })
})

setInterval(() => {
    let waitReplys = document.querySelectorAll('.chat-unreply-time');
    let waitOverReplys = document.querySelectorAll('.chat-unreply-over-time');
    let allWaitReplys = [...waitReplys, ...waitOverReplys]
    chrome.webview.postMessage({
        type: 'newmessage', response: JSON.stringify({
            hasNewMessage: allWaitReplys.length > 0,
            newMessageCount: allWaitReplys.length,
        })
    })
}, 1000)