window.appendBubbleChat = function(chat) {
    const chatlist = document.getElementById("chatList");
    const li = document.createElement("li");
    li.classList.add("d-flex", "mb-3")
    console.log("deneme1234");
    const isUser = chat.userId == document.getElementById("UserID").value;
    if (isUser) {
        li.classList.add("justify-content-end")
    }
    else {
        li.classList.add("justify-content-start")
    }
    li.innerHTML = `
                 <div class="msg ${isUser ? "msg-out" : "msg-in"}">
            <div class="small text-muted mb-1 d-flex ${isUser ? "justify-content-end" : "justify-content-start"}">
                ${new Date(chat.date).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
            </div>
            <div class="msg-text">${chat.messages}</div>
        </div>
                `
    chatlist.appendChild(li);
    chatlist.scrollTop = chatlist.scrollHeight

}