(function() {
    // Load the script
    var script = document.createElement("SCRIPT");
    script.src = 'https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js';
    script.type = 'text/javascript';
    script.onload = function() {
        var $ = window.jQuery;
    };
    document.getElementsByTagName("head")[0].appendChild(script);
})();
(function() {
                                        var rows = document.getElementsByClassName('audio_row_content _audio_row_content');
										for (var i = 0; i < rows.length; i++){
											var row = rows[i];
											var btn = document.createElement("BUTTON");
											$(btn).css("width", "400px")
											var t = document.createTextNode("CLICK ME");
											btn.appendChild(t);
											row.appendChild(btn);
										}
})();