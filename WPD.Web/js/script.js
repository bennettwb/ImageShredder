/* Author: 

*/
var model = {
  currentImages: ko.observableArray([])
}

ko.applyBindings(model);

$(document).ready(function () {

$('#images').everyTime(1000, 'controlled', getNew);

});

function getNew() {
  $.ajax({
    url: 'http://localhost:8080',
    dataType: 'jsonp',
    success: function(data) {
      model.currentImages(data);
    }
  });

}





















