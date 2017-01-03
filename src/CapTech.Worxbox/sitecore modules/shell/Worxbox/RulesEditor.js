function scShowRule(id) {
  $$(".scConditionsActions").each(function(e) { e.hide(); e.previous().removeClassName("scRuleActive"); });

  if (id == null) {
    $("RuleId").value = "";
    return;
  }
  
  var element = $(id);
  element.show();
  element.previous().addClassName("scRuleActive");
  
  $("RuleId").value = id.substr(5);
}

function scMouseOver(sender, event) {
  $(sender).addClassName("scLineHover");
}

function scMouseOut(sender, event) {
  $(sender).removeClassName("scLineHover");
}

function scRuleMouseOver(sender, event) {
  $(sender).addClassName("scRuleHover");
}

function scRuleMouseOut(sender, event) {
  $(sender).removeClassName("scRuleHover");
}

function scFilterConditions(sender, evt) {
  var text = sender.value.toLowerCase();

  $$(".scConditionOption").each(function(e) {
    visible(e.up(), e.innerHTML.toLowerCase().indexOf(text) >= 0);
  });

  $$(".scConditionSection").each(function(e) {
    var isVisible = false;

    e.next().select(".scConditionOption").each(function(o) {
      isVisible |= o.up().visible();
    });

    visible(e, isVisible);
  });
}

function visible(e, isVisible) {
  if (isVisible) {
    e.show();
  }
  else {
    e.hide();
  }
}

function scFilterActions(sender, evt) {
  var text = sender.value.toLowerCase();

  $$(".scActionOption").each(function(e) {
    visible(e.up(), e.innerHTML.toLowerCase().indexOf(text) >= 0);
  });

  $$(".scActionSection").each(function(e) {
    var isVisible = false;

    e.next().select(".scActionOption").each(function(o) {
      isVisible |= o.up().visible();
    });

    visible(e, isVisible);
  });
}

function scFocus(sender, evt) {
  sender.style.color = "#000000";

  if (!sender.isValue) {
    sender.watermark = sender.value;
    sender.value = "";
    sender.isValue = false;
  }
}

function scBlur(sender, evt) {
  if (sender.value == "") {
    sender.isValue = false;
    sender.value = sender.watermark;
    sender.style.color = "#999999";
  }
  else {
    sender.isValue = true;
  }
}
function scToggleSection(sender, evt) {
  if ($(sender).up().next().visible()) {
    $(sender).addClassName("scSectionClosed");
  }
  else {
    $(sender).removeClassName("scSectionClosed");
  }
  $(sender).up().next().toggle();
}