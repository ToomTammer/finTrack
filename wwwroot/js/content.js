$(document).ready(function() {

    $('#deposit button').on("click",function() {
        const amount = parseFloat($('#deposit-amount').val());
        if (isNaN(amount) || amount <= 0) {
            alert('Please enter a valid amount.');
            return;
        }
        
        let data = {
            Amount: amount,
            Action: "Deposit",
        }

        PostConfirmation(data);
        
    });

    $('#withdraw button').on("click",function() {
        const amount = parseFloat($('#withdraw-amount').val());
        if (isNaN(amount) || amount <= 0) {
            alert('Please enter a valid amount.');
            return;
        }

        let data = {
            Amount: amount,
            Action: "Withdraw",
        }
        
        PostConfirmation(data);

    });

    $('#receive .nav-link').on("click",function(e) {
        e.preventDefault();
        updateTransactionHistory();
        
    });

    $('#transfer button').on("click",function() {
        const amount = parseFloat($('#transfer-amount').val());
        const recipient = $('#transfer-to').val();
        if (isNaN(amount) || amount <= 0 || !recipient) {
            alert('Please enter a valid amount and recipient.');
            return;
        }

        let data = {

            Amount: amount,
            Action: "Transfer",
            ToUserName: recipient
        }
        
        PostConfirmation(data);
        
    });

    $("#pagination").on("submit", function (e) {
        e.preventDefault();
        updateTransactionHistory();
    });

    $("#page-link").on("click", function (e) {
        e.preventDefault();
        updateTransactionHistory();
    });

    
    updateTransactionHistory('transfer')

});



function updateTransactionHistory() {
    let data = {
        menuType: $('#receive .nav-link.active').data("value"),
        PageNumber: $('#page').val(),
        Action:"Transfer"
    }
    console.log(data);

    $.ajax({
        url: '/Transaction/GetTransactionsHistory', 
        type: 'GET',
        contentType: 'application/json',
        data: data,
        success: function(response) {
            const table = $('#transaction-history');
            table.empty(); 
            table.html(response);
        },
        error: function() {
            alert('Failed to fetch transaction history. Please try again.');
        }
    });
}

function PostConfirmation(dataPost) {

    $.ajax({
        url: '/Transaction/confirmation', 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataPost),
        success: function(response) {
            if(response.success)
            {
                var msgSwal = "";
                if(dataPost.Action =="Deposit")
                {
                    msgSwal = "Do you want to deposit $" + response.amount
                }
                else if(dataPost.Action == "Withdraw")
                {
                    msgSwal = "Do you want to Withdraw THB" + response.amount
                }
                else if(dataPost.Action == "Transfer")
                {
                    msgSwal = "Do you want to Transferred THB" + response.amount  + " to " + response.toUser
                }

                Swal.fire({
                    title: "Are you sure?",
                    text: msgSwal,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes"
                  }).then((result) => {
                    if (result.isConfirmed) {
                        PostTransaction(dataPost)
                    }
                  });
            }else
            {
                Swal.fire({
                    title: "Failed",
                    text: response.message,
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }
        },
        error: function(err) {
            Swal.fire({
                title: "Failed",
                text: err.responseJSON.message,
                icon: "error",
                confirmButtonText: "OK"
            });
        }
    });
}

function PostTransaction(dataPost) {
    $.ajax({
        url: '/Transaction/create', 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataPost),
        success: function(response) {
            if(response.success)
            {
                Swal.fire({
                    title: "Transaction Successful!",
                    text: "The transaction was completed successfully.",
                    icon: "success",
                    confirmButtonText: "OK"
                });

                updateTransactionHistory('Transfer');
            }
            else{
                Swal.fire({
                    title: "Transaction Failed",
                    text: response.message,
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }  
        },
        error: function(err) {
            Swal.fire({
                title: "Transaction Failed",
                text: err.responseJSON.message,
                icon: "error",
                confirmButtonText: "OK"
            });
        }
    });
}


