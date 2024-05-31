import { check, sleep } from 'k6';
import http from 'k6/http';

const userId = 'ab497ca3-4304-47fd-866d-b647f4daaa99';

export default () => {
    const payload = JSON.stringify({
        "userId": userId,
        "message": "Hello World!"
    });

    const resPost = http.post(
        'http://localhost:8089/notifications',
        payload,
        {
            headers: {
                'Content-Type': 'application/json',
            }
        });

    check(resPost, {
        'is status 202': (r) => r.status === 202,
    });


    const resGet = http.get(`http://localhost:8088/users/${userId}/total-notifications`);
    check(resGet, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(1);
}
