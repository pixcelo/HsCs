// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// API呼び出しテスト
const getSample = document.getElementById("get-sample");
getSample.addEventListener("click", async () => {
    console.log("testGet");

    // API呼び出し
    const response = await fetch("https://localhost:7037/api/sample/get");
    const data = await response.json();
    console.log(data);

});

const postSample = document.getElementById("post-sample");
postSample.addEventListener("click", async () => {
    console.log("testPost");
});