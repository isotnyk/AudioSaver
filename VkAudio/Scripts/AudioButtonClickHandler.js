window.onclick = function(e) {
    e.preventDefault();
    var audioDataDiv = null;
    var currentClass = e.target.getAttribute('class');
    if (currentClass == 'audio_row__title _audio_row__title') {
        audioDataDiv = e.target.parentNode.parentNode.parentNode.parentNode;
    } else if (currentClass == 'audio_row__performer_title') {
        audioDataDiv = e.target.parentNode.parentNode.parentNode;
    } else if (currentClass == 'audio_row__inner' || currentClass == 'audio_row__cover_icon _audio_row__cover_icon') {
        audioDataDiv = e.target.parentNode.parentNode;
    } else if (currentClass == 'audio_row_content _audio_row_content') {
        audioDataDiv = e.target.parentNode;
    }

    bound.onClicked(audioDataDiv.getAttribute('data-audio'));
}