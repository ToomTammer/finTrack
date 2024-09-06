$(document).ready(() => {
    $('#loginForm').on('submit', async (event) => {
        event.preventDefault(); // Prevent the default form submission
        
        let data = {
            UserName: $('#username').val(),
            Password: $('#password').val()
        };


        try {
            $.ajax({
                url: '/Account/Login',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data), 
                success: function(response) {
                    window.location.href = '/Home/Content';
                },
                error: function(xhr) {
                    console.error('Error response:', xhr.responseText);
                    var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'An unexpected error occurred.';
                    $('#message').text('Error: ' + xhr.responseText).css('color', 'red');
                }
            });
        }catch (error) {
            console.error('Error:', error);
            $('#message').text('An unexpected error occurred.').css('color', 'red');
        }
        finally {
            $('#spinner').hide();
        }
    });
});
