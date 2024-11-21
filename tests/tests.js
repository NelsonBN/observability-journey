import { check, sleep } from 'k6';
import http from 'k6/http';

let delta = 200;

export let options = {
    stages:[
        { duration: '5s', target: (delta * 1) },
        { duration: '20s', target: (delta * 1) },
        { duration: '5s', target: (delta * 2) },
        { duration: '20s', target: (delta * 2) },
        { duration: '5s', target: (delta * 3) },
        { duration: '20s', target: (delta * 3) },
        { duration: '5s', target: (delta * 4) },
        { duration: '20s', target: (delta * 4) },
        { duration: '5s', target: (delta * 5) },
        { duration: '20s', target: (delta * 5) },
        { duration: '5s', target: (delta * 6) },
        { duration: '20s', target: (delta * 6) },
        { duration: '5s', target: (delta * 7) },
        { duration: '20s', target: (delta * 7) },
        { duration: '5s', target: (delta * 8) },
        { duration: '20s', target: (delta * 8) },
        { duration: '5s', target: (delta * 9) },
        { duration: '20s', target: (delta * 9) },
        { duration: '5s', target: (delta * 10) },
        { duration: '20s', target: (delta * 10) },
        { duration: '30s', target: 0 }
    ],
    thresholds: {
        'http_req_duration': ['p(95)<500'], // 99% of requests must complete below 500ms
    }
};


const userId = '28111954-9422-4822-a40a-c912f9f297dd';
const notificationId = '5f4dac16-5692-46b9-919b-9b4f3e0d0d91';

const USERS_API_HOST = `${ __ENV.USERS_API_HOST || "http://localhost:8088" }/users`;
const NOTIFICATIONS_API_HOST = `${__ENV.NOTIFICATIONS_API_HOST || "http://localhost:8089" }/notifications`;


function getRandom() {
    const MIN = 1;
    const MAX = 100000000;
    return Math.floor(Math.random() * (MAX - MIN + 1)) + MIN;
}

export default () => {
    const number = getRandom();

    // ********** USERS **********

    // GET: users/:userId
    const usersGet = http.get(`${USERS_API_HOST}/${userId}`);
    check(usersGet, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(0.250);

    // GET: users/:userId/total-notifications
    const usersGetTotals = http.get(`${USERS_API_HOST}/${userId}/total-notifications`);
    check(usersGetTotals, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(0.250);

    // POST: users
    const usersPost = http.post(
        USERS_API_HOST,
        JSON.stringify({
            "name": `Fake User ${number}`,
            "email": `fake.${number}@fake.fk`,
            "phone": `${number}`
        }),
        {
            headers: {
                'Content-Type': 'application/json',
            }
        });
    check(usersPost, {
        'is status 201': (r) => r.status === 201,
    });


    sleep(0.500);


    // ********** NOTIFICATIONS **********

    // GET: notifications/:notificationId
    const notificationsGet = http.get(`${NOTIFICATIONS_API_HOST}/${notificationId}`);
    check(notificationsGet, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(0.250);

    // POST: notifications
    const notificationsPost = http.post(
        NOTIFICATIONS_API_HOST,
        JSON.stringify({
            "userId": userId,
            "body": "Hello World!"
        }),
        {
            headers: {
                'Content-Type': 'application/json',
            }
        });
    check(notificationsPost, {
        'is status 202': (r) => r.status === 202,
    });

    sleep(1);
}
