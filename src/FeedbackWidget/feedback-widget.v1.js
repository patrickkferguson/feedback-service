function uuidv4() {
  return ([1e7]+-1e3+-4e3+-8e3+-1e11).replace(/[018]/g, c =>
    (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
  )
}

const aaa_feedback_state = {
    deviceId: '',
    selectedScore: null,
    ratingId: ''
};

function getDeviceId() {
    const deviceIdCookieKey = 'aaa-feedback-device-id';

    const cookies = document.cookie.split(';');
    const deviceIdCookie = cookies.filter(c => c.indexOf(`${deviceIdCookieKey}=`) >= 0);
    console.log(deviceIdCookie);
    if (deviceIdCookie.length > 0) {
        aaa_feedback_state.deviceId = deviceIdCookie[0].replace(`${deviceIdCookieKey}=`, '');
        console.log(`Found device id: ${aaa_feedback_state.deviceId}`);
    } else {
        aaa_feedback_state.deviceId = uuidv4();
        console.log(`New device id: ${aaa_feedback_state.deviceId}`);
        document.cookie = `${deviceIdCookieKey}=${aaa_feedback_state.deviceId}; expires=Fri, 31 Dec 9999 23:59:59 GMT`;
    }
}

function submitScore() {
    const inputs = document.getElementsByName('aaa-feedback-score');
    
    for (let i = 0; i < inputs.length; i++) {
        if (inputs[i].checked === true) {
            aaa_feedback_state.selectedScore = inputs[i].value;
        }
    }

    if (aaa_feedback_state.selectedScore === null) {
        alert('Please choose a score out of 10.');
        return;
    }

    var addRatingRequest = {
        deviceId: aaa_feedback_state.deviceId,
        pageId: aaa_feedback_pageId,
        score: aaa_feedback_state.selectedScore
    };

    const request = new XMLHttpRequest();
    request.onload = scoreSubmitted;
    request.open('POST', 'http://aaafeedbackapi-327157230.ap-southeast-2.elb.amazonaws.com/ratings');
    request.setRequestHeader('Content-type', 'application/json');
    request.send(JSON.stringify(addRatingRequest));
}

function submitComment() {
    const comment = document.getElementById('aaa-feedback-comment');

    if (comment.value === '') {
        renderThankYou();
        return;
    }

    var addCommentRequest = {
        comment: comment.value
    };

    const request = new XMLHttpRequest();
    request.onload = commentSubmitted;
    request.open('POST', `http://aaafeedbackapi-327157230.ap-southeast-2.elb.amazonaws.com/ratings/${aaa_feedback_state.ratingId}/comments`);
    request.setRequestHeader('Content-type', 'application/json');
    request.send(JSON.stringify(addCommentRequest));
}

function scoreSubmitted() {
    if(this.status === 200 &&
      this.responseText !== null) {
        const response = JSON.parse(this.responseText);
        aaa_feedback_state.ratingId = response.ratingId;
        setCookie();
        renderCommentForm();
    } else {
      console.log(this.responseText);
      hideWidget();
    }
}

function commentSubmitted() {
    if(this.status === 200) {
        renderThankYou();
    } else {
      console.log(this.responseText);
      hideWidget();
    }
}

function showFeedbackForm() {
    const timestampCookieKey = 'aaa-feedback-timestamp';

    const cookies = document.cookie.split(';');
    const timestampCookie = cookies.filter(c => c.indexOf(`${timestampCookieKey}=`) >= 0);
    console.log(timestampCookie);
    if (timestampCookie.length > 0) {
        const timestamp = timestampCookie[0].replace(`${timestampCookieKey}=`, '');
        const timestampDate = new Date(timestamp);
        console.log(`Cookie timestamp: ${timestampDate}`);
        const threshold = new Date();
        threshold.setDate(-180);
        console.log(`Threshold: ${threshold}`);
        return timestamp < threshold;
    } else {
        return true;
    }
}

function setCookie() {
    const now = new Date();
    document.cookie = `aaa-feedback-timestamp=${now}; expires=Fri, 31 Dec 9999 23:59:59 GMT`;
}

function renderScoreForm() {
    if (!showFeedbackForm()) {
        return;
    }

    if (typeof aaa_feedback_pageId === 'undefined') {
        console.error('AAA Feedback Widget - page id not found. Please declare a variable called "aaa_feedback_pageId" and assign the feedback page id for this page.');
        return;
    }

    const body = document.getElementsByTagName('body');
    const widget = document.createElement('div');
    widget.id = 'aaa-feedback-widget';
    widget.style.border = '5px solid lightblue';
    widget.style.padding = '10px';
    widget.style.fontFamily = 'sans-serif';

    const scoreForm = document.createElement('div');
    scoreForm.id = 'aaa-feedback-score-form';

    const widgetHtml = '<div><p>Thank you for using this service. We would appreciate your feedback by answering this question.</p><p>How likely would you be to recommend this service to others?</p></div>';

    scoreForm.innerHTML = widgetHtml;

    const maxScore = 10;
    const scoreTable = document.createElement('table');
    const labelRow = document.createElement('tr');
    const inputRow = document.createElement('tr');
    for (let i = 1; i <= maxScore; i++) {
        const labelCell = document.createElement('td');
        labelCell.align = 'center';
        if (i === 1) {
            labelCell.innerHTML = `${i}<br/>(Not at all)`;
        } else if (i === maxScore) {
            labelCell.innerHTML = `${i}<br/>(Definitely!)`;
        } else {
            labelCell.innerHTML = `${i}<br/>&nbsp;`;
        }
        labelRow.appendChild(labelCell);

        const inputCell = document.createElement('td');
        inputCell.align = 'center';
        inputCell.innerHTML = `<input type="radio" name="aaa-feedback-score" value="${i}"/>`;
        inputRow.appendChild(inputCell);
    }

    scoreTable.appendChild(labelRow);
    scoreTable.appendChild(inputRow);
    scoreForm.appendChild(scoreTable);

    addCloseButton(scoreForm);

    const submitButton = document.createElement('button');
    submitButton.innerText = 'Submit';
    submitButton.onclick = submitScore;

    scoreForm.appendChild(submitButton);
    widget.appendChild(scoreForm);

    document.body.appendChild(widget);
}

function renderCommentForm(score) {
    const widget = document.getElementById('aaa-feedback-widget');

    const scoreForm = document.getElementById('aaa-feedback-score-form');
    scoreForm.style.display = 'none';

    const commentForm = document.createElement('div');
    commentForm.id = 'aaa-feedback-comment-form';
    
    const selectedScore = parseInt(aaa_feedback_state.selectedScore, 10);

    if (selectedScore <= 4) {
        commentForm.innerHTML = '<p>What can we do to improve?</p>';
    } else if (selectedScore >= 9) {
        commentForm.innerHTML = '<p>Is there anything you particularly like?</p>';
    } else {
        commentForm.innerHTML = '<p>Is there anything we can do to improve?</p>';
    }

    const textArea = document.createElement('textarea');
    textArea.id = 'aaa-feedback-comment'
    commentForm.appendChild(textArea);

    commentForm.appendChild(document.createElement('br'));

    addCloseButton(commentForm);

    const submitButton = document.createElement('button');
    submitButton.innerText = 'Submit';
    submitButton.onclick = submitComment;

    commentForm.appendChild(submitButton);
    widget.appendChild(commentForm);
}

function renderThankYou() {
    const widget = document.getElementById('aaa-feedback-widget');

    const commentForm = document.getElementById('aaa-feedback-comment-form');
    commentForm.style.display = 'none';

    const thankYou = document.createElement('div');
    thankYou.innerHTML = '<p>Thank you for your feedback.</p>';

    widget.appendChild(thankYou);
    widget.appendChild(document.createElement('br'));
    addCloseButton(widget);
}

function addCloseButton(parent) {
    const closeButton = document.createElement('button');
    closeButton.innerText = 'Close';
    closeButton.onclick = hideWidget;
    closeButton.style.marginRight = '10px';
    parent.appendChild(closeButton);
}

function hideWidget() {
    const widget = document.getElementById('aaa-feedback-widget');
    widget.style.display = 'none';
}

getDeviceId();

window.addEventListener('load', renderScoreForm);