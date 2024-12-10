import smtplib
from flask import Flask, render_template, request
from email.message import EmailMessage

app = Flask(__name__)


@app.route('/mailSent', methods=['GET', 'POST'])
def mailSent():
    return render_template('mailSent.html')


@app.route('/', methods=['GET', 'POST'])
@app.route('/home', methods=['GET', 'POST'])
def home():
    content = request.args.get('content')
    subject = request.args.get('subject')
    fromDest = request.args.get('from')
    toDest = request.args.get('to')

    msg = EmailMessage()
    msg.set_content(content)
    msg['Subject'] = subject
    msg['From'] = fromDest
    msg['To'] = toDest

    server = smtplib.SMTP_SSL('smtp.gmail.com', 465)
    server.login("YourLogin", "Password")
    server.send_message(msg)
    server.quit()

    return render_template('index.html')


@app.errorhandler(404)
def page_not_found(e):
    return render_template('404.html'), 404


if __name__ == '__main__':
    app.run(debug=True)
