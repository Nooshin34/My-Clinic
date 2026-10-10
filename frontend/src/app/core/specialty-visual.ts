export type SpecialtyKind =
  | 'heart'
  | 'child'
  | 'bone'
  | 'skin'
  | 'eye'
  | 'ear'
  | 'women'
  | 'brain'
  | 'tooth'
  | 'surgery'
  | 'emergency'
  | 'mind'
  | 'lung'
  | 'stomach'
  | 'kidney'
  | 'gland'
  | 'shield'
  | 'scan'
  | 'mask'
  | 'internal'
  | 'general';

export interface SpecialtyVisual {
  kind: SpecialtyKind;
  color: string;
  soft: string;
}

const visuals: Record<SpecialtyKind, SpecialtyVisual> = {
  heart: { kind: 'heart', color: '#d7263d', soft: '#fde7eb' },
  child: { kind: 'child', color: '#e0920a', soft: '#fff3d8' },
  bone: { kind: 'bone', color: '#0d8a82', soft: '#e4f6f4' },
  skin: { kind: 'skin', color: '#d23b8d', soft: '#fde7f3' },
  eye: { kind: 'eye', color: '#0e86c7', soft: '#e4f3fb' },
  ear: { kind: 'ear', color: '#7046d6', soft: '#efe8fb' },
  women: { kind: 'women', color: '#c43170', soft: '#fde7f1' },
  brain: { kind: 'brain', color: '#5746b0', soft: '#ece8fb' },
  tooth: { kind: 'tooth', color: '#2b8ed4', soft: '#e5f3fb' },
  surgery: { kind: 'surgery', color: '#c4452d', soft: '#fdece8' },
  emergency: { kind: 'emergency', color: '#e25b12', soft: '#ffefe4' },
  mind: { kind: 'mind', color: '#6a4fd0', soft: '#eeeafb' },
  lung: { kind: 'lung', color: '#148f86', soft: '#e4f6f4' },
  stomach: { kind: 'stomach', color: '#d56a1c', soft: '#fff0e3' },
  kidney: { kind: 'kidney', color: '#2a9a46', soft: '#e6f6eb' },
  gland: { kind: 'gland', color: '#b07d08', soft: '#fbf3dc' },
  shield: { kind: 'shield', color: '#6b8f16', soft: '#f1f6e3' },
  scan: { kind: 'scan', color: '#3d5afe', soft: '#eaeefe' },
  mask: { kind: 'mask', color: '#54657a', soft: '#eef1f5' },
  internal: { kind: 'internal', color: '#1d6fe0', soft: '#e7f0fd' },
  general: { kind: 'general', color: '#3d5a80', soft: '#e8eef6' },
};

const rules: { kind: SpecialtyKind; words: string[] }[] = [
  { kind: 'heart', words: ['قلب', 'عروق', 'کاردیو', 'cardio', 'heart'] },
  { kind: 'child', words: ['کودک', 'اطفال', 'نوزاد', 'pediatric'] },
  { kind: 'bone', words: ['ارتوپد', 'استخوان', 'ortho'] },
  { kind: 'skin', words: ['پوست', 'درمات', 'dermat'] },
  { kind: 'eye', words: ['چشم', 'ophthal'] },
  { kind: 'ear', words: ['گوش', 'حلق', 'بینی', 'ent'] },
  { kind: 'women', words: ['زنان', 'زایمان', 'مامایی', 'obgyn'] },
  { kind: 'brain', words: ['مغز', 'اعصاب', 'نورول', 'neuro'] },
  { kind: 'tooth', words: ['دندان', 'dental'] },
  { kind: 'emergency', words: ['اورژانس', 'emergency'] },
  { kind: 'mind', words: ['روان', 'psych'] },
  { kind: 'lung', words: ['ریه', 'pulmon'] },
  { kind: 'stomach', words: ['گوارش', 'gastro'] },
  { kind: 'kidney', words: ['کلیه', 'اورول', 'urolog', 'nephro'] },
  { kind: 'gland', words: ['غدد', 'endocr'] },
  { kind: 'shield', words: ['عفونی', 'infectious'] },
  { kind: 'scan', words: ['رادیو', 'تصویربرداری', 'radiolog'] },
  { kind: 'mask', words: ['بیهوش', 'anesth'] },
  { kind: 'surgery', words: ['جراحی', 'surgery'] },
  { kind: 'internal', words: ['داخلی', 'internal'] },
];

export function specialtyVisual(value: string): SpecialtyVisual {
  const text = value.trim().toLowerCase().replace(/\u200c/g, '');
  const match = rules.find((rule) => rule.words.some((word) => text.includes(word)));
  return match ? visuals[match.kind] : visuals.general;
}
