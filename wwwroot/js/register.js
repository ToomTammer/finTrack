$(document).ready(function() {
    $('#registerForm').on('submit', function(event) {
        event.preventDefault(); 
        $('#spinner').show();

        let data = {
            email: $('#email').val(),
            username: $('#username').val(),
            firstName : $('#firstName').val(),
            lastName : $('#lastName').val(),
            password: $('#password').val()
        };

        console.log('Sending data:', data);
        
        try {
            $.ajax({
                url: '/Account/Register',
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
