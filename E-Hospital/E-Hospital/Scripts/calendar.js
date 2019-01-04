//var events = [];
//$(".events").each(function () {
//    var title = $(".title", this).text().trim();
//    var start = $(".start", this).text().trim();
//    var event = {
//        "title": title,
//        "start": start
//    };
    
//    events.push(event);
//});
//Console.log(events);
//$("#calendar").fullCalendar({
//    locale: 'au',
//    events: events,
//    dayClick: function(date, allDay, jsEvent, view) {
//        var d = new Date(date);
//        var m = moment(d).format("DD/MM/YYYY");
//        m = encodeURIComponent(m);
//        var uri = "/Home/Create?date=" + m;
//        $(location).attr('href', uri);
    
//    },
//    eventClick: function()
//});

  var events = [];
    $.ajax({
        type: "GET",
        url: '@Url.Action("GetEvents","ReservationsController")',
        
        success: function (data) {
            $.each(data,
                function (i, v) {
                    events.push({
                        title: v.Date,
                        description: v.Description,
                        start: moment(v.Date)
                    });
                });

            GenerateCalender(events);
        },
        error: function (error) {
            alert('failed');
        }
    });

    function GenerateCalender(events) {
        $('#calender').fullCalendar('destroy');
        $('#calender').fullCalendar({
            contentHeight: 400,
            defaultDate: new Date(),
            timeFormat: 'h(:mm)a',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay,agenda'
            },
            eventLimit: true,
            eventColor: '#378006',
            events: events,
            eventClick: function(calEvent, jsEvent, view) {
                $('#myModal #eventTitle').text(calEvent.title);
                var $description = $('<div/>');
                $description.append($('<p/>').html('<b>Start:</b>' + calEvent.start.format("DD-MM-YYYY")));

                $description.append($('<p/>').html('<b>Description:</b>' + calEvent.description));
                $('#myModal #pDetails').empty().html($description);

                $('#myModal').modal();
            }
        });
    }