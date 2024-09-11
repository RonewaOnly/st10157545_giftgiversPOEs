// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById('subscribeForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const email = document.getElementById('email').value;
    const messageDiv = document.getElementById('subscriptionMessage');

    fetch('/Subscription/Subscribe', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: email })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                messageDiv.textContent = data.message;
                messageDiv.style.color = 'green';
            } else {
                messageDiv.textContent = data.message;
                messageDiv.style.color = 'red';
            }
        })
        .catch(error => {
            messageDiv.textContent = 'An error occurred. Please try again.';
            messageDiv.style.color = 'red';
        });
});