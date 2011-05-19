/* Author: 

*/
var model = {
  currentImages: ko.observableArray([]),
  refreshEnabled: ko.observable('false'),
  currentItem: ko.observable(),
  templateQuery: ko.observable(''),
  templates: ko.observableArray([])
};

ko.dependentObservable(function() {
  var search = this.templateQuery();
 
  $.ajax({
    url: 't',
    type: 'post',
    data: this.templateQuery(),
    dataType: 'json',
    success: function(data) {
      model.templates(data);
    }
  });
}, model);

model.setCurrent= function(data) {
      this.currentItem(data);
};

model.next = function() {
    var idx = model.currentImages().indexOf(model.currentItem());
    if (idx == model.currentImages().length - 1)
        idx = 0;
    model.setCurrent(model.currentImages()[idx + 1]);
};

model.back = function () {
    var idx = model.currentImages().indexOf(model.currentItem());
    if (idx == 0)
        idx = model.currentImages().length;

    model.setCurrent(model.currentImages()[idx - 1]);
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
getNew();
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
      preload(data);
    }
  });
}

function preload(data) {
  var images = new Array();
  for(i = 0; i < data.length; i++)
  {
    images[i] = new Image();
    images[i].src = 'mi/' + data[i].Id + '_600.jpg';
  }
};

function formatDate(datetime) {
    var dateObj = new Date(datetime);
    var dateStr = (dateObj.getMonth()+1) + "/" + dateObj.getDate() + "/" + dateObj.getFullYear();
    return dateStr; // will return mm/dd/yyyy
}





















