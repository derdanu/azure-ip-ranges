// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function(){
    // Initialize select2
    $('#selST').select2();

    function urlEnc(s) {
      return s.replace(/:/g, '%3A')
        .replace(/\//g, '%2F')
        .replace(/\?/g, '%3F')
        .replace(/=/g, '%3D');
    }

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

            $('#result').html('<textarea id="resultarea" class="form-control" rows="' + msg.addressPrefixes.length + '"></textarea>'); 

            for (var i = 0; i < msg.addressPrefixes.length ; i++) {
                $('#resultarea').append(msg.addressPrefixes[i] + '\n');
            }
                        
            var armdownloadlink = 'downloadARMTemplate/' + $('#armdownloadlink').data('url') + '/' + id;
            $('#downloadlink').html('<a href=/Home/getPrefixes?id=' + id + '>JSON Download</a>');
            $('#armdownloadlink').html('<a href=' + armdownloadlink + '>ARM Download</a>');
            $('#deploytoazure').html('<a href="https://portal.azure.com/#create/Microsoft.Template/uri/' + urlEnc(window.location.origin) + urlEnc('/') + urlEnc(armdownloadlink) + '">Deploy to Azure</a>');
            $('#opensenselink').html('<a target=_blank href=/getOPNSenseURLTable/' + $('#opensenselink').data('url') + '/' + id + '>OPNsense UrlTable</a>');

          });
           
          request.fail(function( jqXHR, textStatus ) {
            alert( "Request failed: " + textStatus );
          });
          

    });
    
});


