__workbench__.workbenchOptions.extra.get(`/backstage/currentuser`).then(res => {
    chrome.webview.postMessage({
        type: 'currentuser', response: JSON.stringify({
            userName: res.data.CustomerServiceInfo.screen_name,
            userId: res.data.CustomerServiceInfo.id,
            avatar: res.data.CustomerServiceInfo.avatar_url
        })
    })
})

setInterval(() => {
    let waitReplys = document.querySelectorAll('.lmMoDu1yDLhUhNlOi0AS')
    chrome.webview.postMessage({
        type: 'newmessage', response: JSON.stringify({
            hasNewMessage: waitReplys.length > 0,
            newMessageCount: waitReplys.length,
        })
    })
}, 1000)