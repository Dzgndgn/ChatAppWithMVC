"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
//console.log(userID)
//connection.on("notifyAll", function (userID) {
//    console.log("Kullanıcı bildirimi geldi:", userID);
console.log("dshsf")
var userID = document.getElementById("UserID").value
console.log(userID)
connection.start().then(() => connection.invoke("Connect", userID))
//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});
connection.on("Messages", function (chat) {
    window.appendBubbleChat(chat);
});


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function (e) {
    e.preventDefault(); // GET isteğini engelle
   
    const messagedto = {
        UserId: document.getElementById("UserID").value,
        message: document.getElementById("messageInput").value,
        receivedId: document.getElementById("Receiver").value //burayı dolduracağım
    }
    fetch(window.sendUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value // antiforgery kullanıyorsan
        },
        body: JSON.stringify(messagedto),
        credentials: "same-origin"
    })
        .then(res => {
            if (!res.ok) throw new Error("Gönderim başarısız! " + res.status);
            return res.json();
            console.log("Mesaj gönderildi!");
        })
        .then(chat => {
            window.appendBubbleChat(chat);
        })
        .catch(err => console.error(err));
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});