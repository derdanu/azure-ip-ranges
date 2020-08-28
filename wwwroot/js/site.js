// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function(){
    // Initialize select2
    $('#selST').select2();

    // Read selected option
    $('#selST').change(function(){
        var id = $('#selST option:selected').text();
        //var val = $('#selST').val();
        
        var request = $.ajax({
            url: "/Home/getPrefixes",
            method: "GET",
            data: { id : id },
            dataType: "html"
          });
           
          request.done(function( rsp ) {
            
            var msg = $.parseJSON(rsp);

            $('#result').html('<textarea id="resultarea" class="form-control" rows="' + msg.length + '"></textarea>'); 

            for (var i = 0; i < msg.length ; i++) {
                $('#resultarea').append(msg[i] + '\n');
            }
                        

            $('#downloadlink').html('<a target=_blank href=/Home/getPrefixes?id=' + id + '>JSON Download</a>');
            $('#opensenselink').html('<a target=_blank href=/Home/getOPNSenseURLTable?id=' + id + '>OPNsense UrlTable</a>');

          });
           
          request.fail(function( jqXHR, textStatus ) {
            alert( "Request failed: " + textStatus );
          });
          

    });
    
});


