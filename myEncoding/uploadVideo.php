<?php

save_video();

function save_video()
{
    file_put_contents("post.log", print_r(json_decode($_POST["application/json"], true)['id'], true));


    $allowedExts = ['jpg', 'jpeg', 'gif', 'png', 'mp3', 'mp4', 'wma', 'mkv'];
    $extension = pathinfo($_FILES['file']['name'], PATHINFO_EXTENSION);

    if (
        ($_FILES['file']['type'] == 'video/mp4' ||
            $_FILES['file']['type'] == 'audio/mp3' ||
            $_FILES['file']['type'] == 'audio/wma' ||
            $_FILES['file']['type'] == 'video/x-matroska' ||
            $_FILES['file']['type'] == 'image/gif' ||
            $_FILES['file']['type'] == "application/octet-stream" ||
            $_FILES['file']['type'] == 'image/jpeg') &&
        $_FILES['file']['size'] < 2000000000 &&
        in_array($extension, $allowedExts)
    ) {
        if ($_FILES['file']['error'] > 0) {
            echo 'Return Code: ' . $_FILES['file']['error'] . '<br />';
        } else {
            // echo 'Upload: ' . $_FILES['file']['name'] . '<br />';
            // echo 'Type: ' . $_FILES['file']['type'] . '<br />';
            // echo 'Size: ' . $_FILES['file']['size'] / 1024 . ' Kb<br />';
            // echo 'Temp file: ' . $_FILES['file']['tmp_name'] . '<br />';

            // if (
            //     file_exists('/var/www/html/upload/' . $_FILES['file']['name'])
            // ) {
            //     echo $_FILES['file']['name'] . ' already exists. ';
            // } else {
            move_uploaded_file($_FILES['file']['tmp_name'], '/var/www/html/upload/' . $_FILES['file']['name']);

            $uploadedFilePath = '/var/www/html/upload/' . $_FILES['file']['name'];

            echo 'Stored in: ' . $uploadedFilePath;
            echo "<br>";
            echo substr(str_replace("/var/www/html/upload/", "", $uploadedFilePath), 0, 10);
            echo "<br>";
            echo "uuid dir = " . strtoupper(sha1(substr(str_replace("/var/www/html/upload/", "", $uploadedFilePath), 0, 10)));;
            ask_video_conversion($uploadedFilePath, $extension, json_decode($_POST["application/json"], true)['id']);
            // }
        }
    } else {
        http_response_code(500);
        echo 'Invalid file';
    }
}

function ask_video_conversion($uploadedFilePath, $extension, $userID)
{
    $ch = curl_init();

    curl_setopt($ch, CURLOPT_URL, 'http://127.0.0.1/convertVideo.php');
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_TIMEOUT, 1); //timeout in seconds
    curl_setopt(
        $ch,
        CURLOPT_POSTFIELDS,
        'uploadedFilePath=' . $uploadedFilePath . '&extension=' . $extension . '&userId=' . $userID
    );

    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

    $server_output = curl_exec($ch);

    curl_close($ch);

}
