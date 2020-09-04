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
        .replace(/=/g, '%3D')
        .replace(/;/g, '%3B');
    }

    // Read selected option
    $('#selST').change(function(){
        //var id = $('#selST option:selected').text();
        //var val = $('#selST').val();
 
        var id = '';
        $.each( $('#selST').val(), function( index, value ){
          console.log(index + ": " + value );
          id += value
          if (index < $('#selST').val().length - 1 ) { id += ';'; }
        });
       
        var request = $.ajax({
            url: "/getPrefixes/" + $('#downloadlink').data('url') + '/' + id,
            method: "GET",
            dataType: "html"
          });
           
          request.done(function( rsp ) {
            
            var json = $.parseJSON(rsp);
            var servicecnt = 0;
            var ranges = [];
           
            $.each(json, function ( index, service) {
              servicecnt += service.addressPrefixes.length;
              ranges.push(service.addressPrefixes);
            });
            if (servicecnt > 0) {
              $('#result').html('<p>Found: ' + servicecnt + ' matching IP ranges out of ' + json.length + ' services</p>');
              if (servicecnt > 400)  $('#result').append('<p class="text-danger">UDRs in Azure have a maximum limit of 400 routes</p>');
              $('#result').append('<textarea id="resultarea" class="form-control" rows="' + servicecnt + '"></textarea>'); 

              $.each(ranges, function (index, prefixes) {
                  $.each(prefixes, function (index2, prefix) {
                    $('#resultarea').append(prefix + '\n');
                  });
                  
              });
 
              $('#downloadlink').html('<a class="btn btn-light btn-block" href=getPrefixes/' + $('#downloadlink').data('url') + '/' + id + '>JSON Download</a>');
              $('#armdownloadlink').html('<a class="btn btn-light btn-block" href=downloadARMTemplate/' + $('#armdownloadlink').data('url') + '/' + id + '>ARM Download</a>');
              $('#deploytoazure').html('<a class="btn btn-light btn-block" target=_blank href="https://portal.azure.com/#create/Microsoft.Template/uri/' + urlEnc(window.location.origin) + urlEnc('/') + urlEnc('deployARMTemplate/' + $('#deploytoazure').data('url') + '/' + id) + '">Deploy UDR to Azure</a>');
              $('#opensenselink').html('<a class="btn btn-light btn-block" target=_blank href=/getOPNSenseURLTable/' + $('#opensenselink').data('url') + '/' + id + '>OPNsense UrlTable</a>');
            } else {
              $('#links').each(function( index ) {
                $(this).html('');
              });
              $('#result').html('');
              $('#resultarea').html('');
            }
            

          });
           
          request.fail(function( jqXHR, textStatus ) {
            alert( "Request failed: " + textStatus );
          });
          

    });
    
});


