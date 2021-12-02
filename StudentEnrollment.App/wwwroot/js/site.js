// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
     var PlaceHolderElement = $('#PlaceHolderHere');
     $('button[data-toggle="ajax-modal"]').click(function (event) {
     
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent
        $.get(url).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        })
     
     })


     PlaceHolderElement.on('click', '[data-save="modal"]', function (event){
        event.preventDefault();
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        $.post(actionUrl. sendData).done(function (data)
        {
            PlaceHolderElement.find('.modal').modal('hide');
        })


     })
    




    })


    $(function() {

        $("#AddDepartmentForm").validate({
          rules: {
            pName: {
              required: true,
              minlength: 8
            }
          },
          messages: {
            pName: {
              required: "Please enter some data",
              minlength: "Your data must be at least 8 characters"
            }
          }
        });
      });

$("#btnDelete").click(function()
{
  var _id = $("#deptId").val();

  $.ajax({
    type:"POST",
    url:"/Departments/Delete",
    data:{id: _id},
    success: function(result)
    {
      if(result)
      {
        $("#deleteModal").modal('hide');
        $("#deptId").val(null);
      }
      else{
        alert("Error processing your request");
      }
    }
  })

  var Confirm = function (id)
  {
    $("#deleteModal").modal('show');
  }
    
  });

  
