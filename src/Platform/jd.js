fetch('https://dongdong.jd.com/workbench/checkin.json?version=2.6.3&client=openweb').then(res => {
    res.json().then(r => {
        chrome.webview.postMessage({
            type: 'currentuser', response: JSON.stringify({
                userName: r.data.pin,
                userId: r.data.venderId,
                avatar: r.data.waiterAvatar
            })
        })
    })
})

setInterval(() => {
    let waitReplys = document.querySelectorAll('.user-none-reply-t');
    chrome.webview.postMessage({
        type: 'newmessage', response: JSON.stringify({
            hasNewMessage: waitReplys.length > 0,
            newMessageCount: waitReplys.length,
        })
    })
}, 1000)