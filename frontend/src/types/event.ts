export interface Zone {
  name: string;
  price: number;
  capacity: number;
}

export interface CreateEventRequest {
  name: string;
  date: string;
  venue: string;
  zones: Zone[];
}

export interface CreateEventResponse {
  id: string;
  name: string;
  date: string;
  venue: string;
  status: string;
  zones: (Zone & { id: string })[];
}
