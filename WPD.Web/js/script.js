/* Author: 

*/
var model = {
  currentImages: ko.observableArray([]),
  refreshEnabled: ko.observable('false'),
};
model.toggleRefresh= function() {
    if(this.refreshEnabled() == 'true') {
      this.refreshEnabled('false');
      $('#images').stopTime();

    }
    else {
      this.refreshEnabled('true');
      $('#images').everyTime(1000, 'controlled', getNew);

    }
  };

model.refreshText= ko.dependentObservable(function() {
  if(this.refreshEnabled() == 'true')
  {
    return 'Refresh Enabled';
  }
  else {
    return 'Refresh Disabled';
  }

}, model);

ko.applyBindings(model);

$(document).ready(function () {

//  model.refreshEnabled= true;

  // $('#images').everyTime(1000, 'controlled', getNew);
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






















