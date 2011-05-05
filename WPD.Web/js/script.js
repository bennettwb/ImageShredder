/* Author: 

*/
var model = {
  currentImages: ko.observableArray([])
}

ko.applyBindings(model);

$(document).ready(function () {
    $('#images').everyTime(1000, 'controlled', getNew);
 //   getNew();

});

function getNew() {
  $.ajax({
    url: 'mi',
    dataType: 'json',
    success: function(data) {
      model.currentImages(data);
    }
  });

}






















